using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Orko.Portal.Contracts.Declarations;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class UploadAndSendHandler
{
    private readonly PortalDbContext _db;
    private readonly SendToEvrimHandler _sendHandler;
    private readonly ILogger<UploadAndSendHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UploadAndSendHandler(
        PortalDbContext db,
        SendToEvrimHandler sendHandler,
        ILogger<UploadAndSendHandler> logger)
    {
        _db = db;
        _sendHandler = sendHandler;
        _logger = logger;
    }

    /// <summary>
    /// Dosyayi parse edip DB'ye kaydeder ve Evrim'e gonderir.
    /// </summary>
    public async Task<EvrimResponse> HandleAsync(Guid declarationId, UploadAndSendDto dto)
    {
        await HandleParseOnlyAsync(declarationId, dto);
        return await _sendHandler.HandleAsync(declarationId);
    }

    /// <summary>
    /// Dosyayi parse edip DB'ye kaydeder ama Evrim'e gondermez.
    /// Kullanici formu kontrol edip sonra manuel gonderir.
    /// </summary>
    public async Task<EvrimDeclarationRequest> HandleParseOnlyAsync(Guid declarationId, UploadAndSendDto dto)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        if (declaration.SentToEvrim)
            throw new InvalidOperationException("Beyanname zaten Evrim'e gonderilmis.");

        if (string.IsNullOrWhiteSpace(dto.FileContent))
            throw new InvalidOperationException("Dosya icerigi bos.");

        EvrimDeclarationRequest evrimRequest;
        var format = dto.FileFormat?.ToLowerInvariant() ?? "json";

        if (format == "xml")
        {
            evrimRequest = DeserializeXml(dto.FileContent);
        }
        else
        {
            evrimRequest = JsonSerializer.Deserialize<EvrimDeclarationRequest>(
                dto.FileContent, JsonOptions)
                ?? throw new InvalidOperationException("JSON dosyasi parse edilemedi.");
        }

        // Ek dosyalar varsa (sonuc/cevap XML) merge et
        if (dto.AdditionalFiles is { Count: > 0 })
        {
            MergeAdditionalXmlFiles(evrimRequest, dto.AdditionalFiles);
        }

        declaration.DeclarationData = JsonSerializer.Serialize(evrimRequest, JsonOptions);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Dosyadan parse edildi: {FileNumber} | Format: {Format} | EkDosya: {AdditionalCount}",
            declaration.WorkOrder.FileNumber, format, dto.AdditionalFiles?.Count ?? 0);

        return evrimRequest;
    }

    /// <summary>
    /// Ek XML dosyalarini (sonuc/cevap) parse edip EvrimDeclarationRequest'e merge eder.
    /// </summary>
    private void MergeAdditionalXmlFiles(EvrimDeclarationRequest request, List<UploadFileDto> files)
    {
        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file.FileContent)) continue;

            try
            {
                var content = file.FileContent.Trim();

                // Root elementine gore tani
                if (content.Contains("<IslemSonucGetir2Result") || content.Contains("<GidenXML>"))
                {
                    MergeCevapXml(request, content);
                }
                else if (content.Contains("<Response") && content.Contains("<RefID>"))
                {
                    MergeSonucXml(request, content);
                }
                // <Gelen> ise ana XML — zaten FileContent'te parse edildi, atla
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ek XML dosyasi parse edilemedi: {FileName}", file.FileName);
            }
        }
    }

    /// <summary>
    /// Sonuc XML: Selsil yaniti — RefID, GUID
    /// </summary>
    private static void MergeSonucXml(EvrimDeclarationRequest request, string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);
        // Namespace-agnostic arama
        var response = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Response");
        if (response == null) return;

        var refId = response.Elements().FirstOrDefault(e => e.Name.LocalName == "RefID")?.Value?.Trim();
        if (!string.IsNullOrEmpty(refId) && string.IsNullOrEmpty(request.RefId))
            request.RefId = refId;
    }

    /// <summary>
    /// Cevap XML: Tescil sonucu — embedded GidenXML icinden Beyanname_no, Tescil_tarihi
    /// </summary>
    private static void MergeCevapXml(EvrimDeclarationRequest request, string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);

        // GidenXML elementini bul (HTML-encoded XML iceriyor)
        var gidenXmlElement = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "GidenXML");
        if (gidenXmlElement == null) return;

        // HTML decode
        var encodedXml = gidenXmlElement.Value;
        var decodedXml = WebUtility.HtmlDecode(encodedXml);

        var sonucDoc = XDocument.Parse(decodedXml);
        XNamespace sonucNs = "http://tempuri.org/";
        var sonuc = sonucDoc.Root;
        if (sonuc == null) return;

        // Beyanname_no
        var beyannameNo = sonuc.Element(sonucNs + "Beyanname_no")?.Value?.Trim();
        if (!string.IsNullOrEmpty(beyannameNo) && string.IsNullOrEmpty(request.DosyaNo))
            request.DosyaNo = beyannameNo;

        // Tescil_tarihi -> dosyaTarihi
        var tescilTarihi = sonuc.Element(sonucNs + "Tescil_tarihi")?.Value?.Trim();
        if (!string.IsNullOrEmpty(tescilTarihi) && string.IsNullOrEmpty(request.DosyaTarihi))
        {
            // Format: "23/02/2026" -> "2026-02-23T00:00:00"
            if (DateTime.TryParseExact(tescilTarihi, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                request.DosyaTarihi = dt.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }

    private static EvrimDeclarationRequest DeserializeXml(string xmlContent)
    {
        try
        {
            var doc = XDocument.Parse(xmlContent);
            var gelen = doc.Root
                ?? throw new InvalidOperationException("XML root elementi bulunamadi.");

            XNamespace ns = "http://tempuri.org/";
            var beyanname = gelen.Element(ns + "BeyannameBilgi")
                ?? throw new InvalidOperationException("BeyannameBilgi elementi bulunamadi.");

            var rejimKodu = Val(beyanname, ns, "Rejim");
            var ihracat = rejimKodu?.Length > 0 && rejimKodu[0] == '3';

            // Firma bilgisi — fallback: Alici > DigerGonderici > Gonderici
            string? musteriUnvani = null;
            var firmaBilgi = beyanname.Element(ns + "Firma_bilgi");
            if (firmaBilgi != null)
            {
                // Sırasıyla dene: Alici, DigerGonderici, Gonderici
                foreach (var tip in new[] { "Alici", "DigerGonderici", "Gonderici" })
                {
                    var firma = firmaBilgi.Elements(ns + "firma")
                        .FirstOrDefault(f => f.Element(ns + "Tip")?.Value == tip);
                    musteriUnvani = firma?.Element(ns + "Adi_unvani")?.Value?.Trim();
                    if (!string.IsNullOrEmpty(musteriUnvani)) break;
                }
            }

            // Musteri vergi no — fallback: Islem_yapilacak_firma_vergino > Alici_vergi_no
            var musteriVergi = Val(beyanname, ns, "Islem_yapilacak_firma_vergino")
                ?? Val(beyanname, ns, "Alici_vergi_no");

            // Konteyner: HAYIR→0, EVET→1
            var konteynerRaw = Val(beyanname, ns, "Konteyner");
            var konteyner = string.Equals(konteynerRaw, "EVET", StringComparison.OrdinalIgnoreCase) ? "1" : "0";

            // Parse kalemler ve toplam agirlik hesapla
            var kalemlerElement = beyanname.Element(ns + "Kalemler");
            var kalemler = new List<EvrimDeclarationKalem>();
            decimal toplamBrut = 0, toplamNet = 0;

            if (kalemlerElement != null)
            {
                foreach (var kalem in kalemlerElement.Elements(ns + "kalem"))
                {
                    var brut = Dec(kalem, ns, "Brut_agirlik");
                    var net = Dec(kalem, ns, "Net_agirlik");
                    toplamBrut += brut ?? 0;
                    toplamNet += net ?? 0;

                    kalemler.Add(new EvrimDeclarationKalem
                    {
                        DetayNo = Int(kalem, ns, "Kalem_sira_no"),
                        GtipNo = Val(kalem, ns, "Gtip"),
                        MenseiUlke = Val(kalem, ns, "Mensei_ulke"),
                        BrutAgirlik = brut,
                        NetAgirlik = net,
                        IstatistikiMiktar = Dec(kalem, ns, "Istatistiki_miktar"),
                        KalemFiyati = Dec(kalem, ns, "Fatura_miktari"),
                        Miktar = Dec(kalem, ns, "Miktar"),
                        MiktarBirimi = Val(kalem, ns, "Miktar_birimi"),
                        Doviz = Val(kalem, ns, "Fatura_miktarinin_dovizi"),
                        TicariTanim = Val(kalem, ns, "Ticari_tanimi"),
                        Cinsi = Val(kalem, ns, "Cinsi"),
                        Adedi = Val(kalem, ns, "Adedi"),
                        NavlunMiktari = Dec(kalem, ns, "Navlun_miktari"),
                        SigortaMiktari = Dec(kalem, ns, "Sigorta_miktari"),
                        KdvOrani = Val(kalem, ns, "Kdv_orani"),
                    });
                }
            }

            return new EvrimDeclarationRequest
            {
                RefId = gelen.Element("RefID")?.Value?.Trim(),
                Ihracat = ihracat,
                DosyaTipi = ihracat ? "H" : "T",
                RejimKodu = rejimKodu,
                Gumruk = Val(beyanname, ns, "GUMRUK"),
                BasitlestirilmisUsul = Val(beyanname, ns, "Basitlestirilmis_usul"),
                YukBelgeleriSayisi = Int(beyanname, ns, "Yuk_belgeleri_sayisi"),
                KapAdedi = Val(beyanname, ns, "Kap_adedi"),
                TicaretUlkesi = Val(beyanname, ns, "Ticaret_ulkesi"),
                ReferansNo = Val(beyanname, ns, "Referans_no"),
                MusteriVergi = musteriVergi,
                MusteriUnvani = musteriUnvani,
                GidecegiUlke = Val(beyanname, ns, "Gidecegi_ulke"),
                SevkUlkesi = Val(beyanname, ns, "Gidecegi_sevk_ulkesi"),
                CikistakiAracinTipi = Val(beyanname, ns, "Cikistaki_aracin_tipi"),
                CikistakiAracinKimligi = Val(beyanname, ns, "Cikistaki_aracin_kimligi"),
                CikistakiAracinUlkesi = Val(beyanname, ns, "Cikistaki_aracin_ulkesi"),
                TeslimSekli = Val(beyanname, ns, "Teslim_sekli"),
                TeslimYeri = Val(beyanname, ns, "Teslim_yeri"),
                Konteyner = konteyner,
                SinirdakiAracinTipi = Val(beyanname, ns, "Sinirdaki_aracin_tipi"),
                SinirdakiAracinKimligi = Val(beyanname, ns, "Sinirdaki_aracin_kimligi"),
                SinirdakiAracinUlkesi = Val(beyanname, ns, "Sinirdaki_aracin_ulkesi"),
                SinirdakiTasimaSekli = Val(beyanname, ns, "Sinirdaki_tasima_sekli"),
                ToplamFatura = Dec(beyanname, ns, "Toplam_fatura"),
                ToplamFaturaDovizi = Val(beyanname, ns, "Toplam_fatura_dovizi"),
                ToplamNavlun = Dec(beyanname, ns, "Toplam_navlun"),
                ToplamNavlunDovizi = Val(beyanname, ns, "Toplan_navlun_dovizi"),
                ToplamSigorta = Dec(beyanname, ns, "Toplam_sigorta"),
                ToplamSigortaDovizi = Val(beyanname, ns, "Toplam_sigorta_dovizi"),
                ToplamBrutAgirlik = toplamBrut,
                ToplamNetAgirlik = toplamNet,
                YuklemeBosaltmaYeri = Val(beyanname, ns, "Yukleme_bosaltma_yeri"),
                BankaKodu = Val(beyanname, ns, "Banka_kodu"),
                IsleminNiteligi = Val(beyanname, ns, "Islemin_niteligi"),
                Aciklamalar = Val(beyanname, ns, "Aciklamalar"),
                OdemeSekli = Val(beyanname, ns, "Odeme"),
                BeyanSahibiVergiNo = Val(beyanname, ns, "Beyan_sahibi_vergi_no"),
                MusavirVergiNo = Val(beyanname, ns, "Musavir_vergi_no"),
                OlusturanKullanici = Val(beyanname, ns, "Kullanici_kodu"),
                Kalemler = kalemler,
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"GTS XML parse hatasi: {ex.Message}");
        }
    }

    private static string? Val(XElement parent, XNamespace ns, string name)
    {
        var v = parent.Element(ns + name)?.Value?.Trim();
        return string.IsNullOrEmpty(v) ? null : v;
    }

    private static decimal? Dec(XElement parent, XNamespace ns, string name)
    {
        var v = parent.Element(ns + name)?.Value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return decimal.TryParse(v, CultureInfo.InvariantCulture, out var r) ? r : null;
    }

    private static int? Int(XElement parent, XNamespace ns, string name)
    {
        var v = parent.Element(ns + name)?.Value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return int.TryParse(v, out var r) ? r : null;
    }
}
