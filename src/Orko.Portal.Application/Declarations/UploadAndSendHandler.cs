using System.Globalization;
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

        declaration.DeclarationData = JsonSerializer.Serialize(evrimRequest, JsonOptions);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Dosyadan parse edildi: {FileNumber} | Format: {Format}",
            declaration.WorkOrder.FileNumber, format);

        return evrimRequest;
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

            // Alici firma bilgisi
            string? musteriUnvani = null;
            var firmaBilgi = beyanname.Element(ns + "Firma_bilgi");
            if (firmaBilgi != null)
            {
                var aliciFirma = firmaBilgi.Elements(ns + "firma")
                    .FirstOrDefault(f => f.Element(ns + "Tip")?.Value == "Alici");
                musteriUnvani = aliciFirma?.Element(ns + "Adi_unvani")?.Value?.Trim();
            }

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
                MusteriVergi = Val(beyanname, ns, "Islem_yapilacak_firma_vergino"),
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
