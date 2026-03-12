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

        // WorkOrder.FileNumber: XML'de Adi_unvani ve Cadde_s_no da bos gelirse son fallback
        var customerNameFallback = declaration.WorkOrder.FileNumber;

        EvrimDeclarationRequest evrimRequest;
        var format = dto.FileFormat?.ToLowerInvariant() ?? "json";

        if (format == "xml")
        {
            evrimRequest = DeserializeXml(dto.FileContent, customerNameFallback);
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
            declaration.WorkOrder?.FileNumber ?? declarationId.ToString(), format, dto.AdditionalFiles?.Count ?? 0);

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
                // <Response> + <RefID> → sonuc XML, RefID bos gonderilmeli, atla
                // <Gelen> ise ana XML — zaten FileContent'te parse edildi, atla
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ek XML dosyasi parse edilemedi: {FileName}", file.FileName);
            }
        }
    }

    /// <summary>
    /// Cevap XML: Tescil sonucu — embedded GidenXML icinden Beyanname_no, Tescil_tarihi, MuayeneMemuru vb.
    /// </summary>
    private static void MergeCevapXml(EvrimDeclarationRequest request, string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);

        // GidenXML elementini bul (HTML-encoded XML iceriyor)
        var gidenXmlElement = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "GidenXML");
        if (gidenXmlElement == null) return;

        var decodedXml = WebUtility.HtmlDecode(gidenXmlElement.Value);
        var sonucDoc = XDocument.Parse(decodedXml);
        XNamespace sonucNs = "http://tempuri.org/";
        var sonuc = sonucDoc.Root;
        if (sonuc == null) return;

        // Beyanname_no → BeyannameNo (resmi beyanname numarasi, cevap XML'den gelir)
        var beyannameNo = sonuc.Element(sonucNs + "Beyanname_no")?.Value?.Trim();
        if (!string.IsNullOrEmpty(beyannameNo))
        {
            request.BeyannameNo = beyannameNo;
            // DosyaNo bos ise de doldur
            if (string.IsNullOrEmpty(request.DosyaNo))
                request.DosyaNo = beyannameNo;
        }

        // Tescil_tarihi → DosyaTarihi
        var tescilTarihi = sonuc.Element(sonucNs + "Tescil_tarihi")?.Value?.Trim();
        if (!string.IsNullOrEmpty(tescilTarihi) && string.IsNullOrEmpty(request.DosyaTarihi))
        {
            if (DateTime.TryParseExact(tescilTarihi, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                request.DosyaTarihi = dt.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        // KDV oranı → kalem seviyesi (Vergiler listesinden)
        var kdvVergi = sonuc.Element(sonucNs + "Vergiler")?
            .Elements(sonucNs + "Vergi")
            .FirstOrDefault(v => v.Element(sonucNs + "Kod")?.Value == "40");

        if (kdvVergi != null && request.Kalemler != null)
        {
            var kdvOrani = kdvVergi.Element(sonucNs + "Oran")?.Value?.Trim();
            foreach (var kalem in request.Kalemler.Where(k => string.IsNullOrEmpty(k.KdvOrani)))
                kalem.KdvOrani = kdvOrani;
        }

        // İstatistiki kıymet → kalem seviyesi
        var istatistikiKiymetler = sonuc.Element(sonucNs + "Istatistiki_kiymetleri")?
            .Elements(sonucNs + "Istatistiki_Kiymeti")
            .ToList();

        if (istatistikiKiymetler != null && request.Kalemler != null)
        {
            foreach (var ik in istatistikiKiymetler)
            {
                var kalemNoStr = ik.Element(sonucNs + "Kalem_no")?.Value?.Trim();
                if (!int.TryParse(kalemNoStr, out var kalemNo)) continue;

                var kalem = request.Kalemler.FirstOrDefault(k => k.DetayNo == kalemNo);
                if (kalem == null) continue;

                if (decimal.TryParse(
                    ik.Element(sonucNs + "Miktar")?.Value?.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out var kiymet))
                {
                    kalem.FobTutar = kiymet;  // istatistiki kıymet fob tutar olarak saklanır
                }
            }
        }

        // e-Ticaret sorusu cevabı
        var eTicaretCevap = sonuc.Element(sonucNs + "Soru_cevap")?
            .Elements(sonucNs + "Soru_Cevap")
            .FirstOrDefault(sc => sc.Element(sonucNs + "Soru_no")?.Value == "5178")?
            .Element(sonucNs + "Cevap")?.Value?.Trim();

        // Sonraki kullanım için aciklamalar alanına eklenebilir (opsiyonel)
        if (!string.IsNullOrEmpty(eTicaretCevap) && request.Aciklamalar != null)
        {
            _  = eTicaretCevap; // şimdilik ignore, gerekirse kullan
        }
    }

    private static EvrimDeclarationRequest DeserializeXml(string xmlContent, string? customerNameFallback = null)
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
            // Rejim 3xxx → ihracat, 4xxx/5xxx → ithalat
            var ihracat = rejimKodu?.Length > 0 && rejimKodu[0] == '3';

            // ── Müşteri ünvanı: Önce Firma_bilgi > Adi_unvani dene (Alici > DigerGonderici > Gonderici sırası)
            // Yoksa ilk firmanın Cadde_s_no'sundan al (adres içinde genellikle firma adı olur)
            // Son fallback: WorkOrder.CustomerName (DB'den)
            string? musteriUnvani = null;
            var firmaBilgi = beyanname.Element(ns + "Firma_bilgi");
            if (firmaBilgi != null)
            {
                foreach (var tip in new[] { "Alici", "DigerGonderici", "Gonderici" })
                {
                    var firma = firmaBilgi.Elements(ns + "firma")
                        .FirstOrDefault(f => f.Element(ns + "Tip")?.Value == tip);
                    musteriUnvani = firma?.Element(ns + "Adi_unvani")?.Value?.Trim();
                    if (!string.IsNullOrEmpty(musteriUnvani)) break;
                }
            }

            // Fallback 1: İlk firmanın adresi (bazı XML'lerde Adi_unvani boş gelir)
            if (string.IsNullOrEmpty(musteriUnvani))
            {
                var ilkFirma = firmaBilgi?.Elements(ns + "firma").FirstOrDefault();
                var cadde = ilkFirma?.Element(ns + "Cadde_s_no")?.Value?.Trim();
                if (!string.IsNullOrEmpty(cadde))
                {
                    // Adres formatı: "RO 25215714  STR POVERNEI NR..." → ilk anlamlı kısmı al (max 100 karakter)
                    musteriUnvani = cadde.Length > 100 ? cadde[..100] : cadde;
                }
            }

            // Fallback 2: WorkOrder.CustomerName (DB'de iş emri oluşturulurken kaydedilmiş olan)
            if (string.IsNullOrEmpty(musteriUnvani) && !string.IsNullOrEmpty(customerNameFallback))
                musteriUnvani = customerNameFallback;

            // ── Müşteri vergi no
            var musteriVergi = Val(beyanname, ns, "Islem_yapilacak_firma_vergino")
                ?? Val(beyanname, ns, "Alici_vergi_no");

            // ── Konteyner: HAYIR→"0", EVET→"1"
            var konteynerRaw = Val(beyanname, ns, "Konteyner");
            var konteyner = string.Equals(konteynerRaw, "EVET", StringComparison.OrdinalIgnoreCase) ? "1" : "0";

            // ── Kalemler
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

                    // FaturaTarihiSayisi: "27/02/2026 GTSEXT1221" → tarih + no ayrıştır
                    var faturaTarihiSayisi = Val(
                        beyanname.Element(ns + "KiymetBildirim")?.Element(ns + "Kiymet"),
                        ns, "FaturaTarihiSayisi");
                    var (parsedFaturaNo, parsedFaturaTarihi) = ParseFaturaTarihiSayisi(faturaTarihiSayisi);

                    // Ödeme şekilleri
                    var odemeSekilleri = kalem
                        .Element(ns + "OdemeSekilleri")?
                        .Elements(ns + "OdemeSekli")
                        .Select(os => new EvrimOdemeSekli
                        {
                            Kodu   = Int(os, ns, "OdemeSekliKodu"),
                            Tutari = Dec(os, ns, "OdemeTutari"),
                            TranferBildirimNo = Val(os, ns, "TBFID"),
                        })
                        .ToList();

                    // Dokümanlar (belgeler)
                    var dokumanlar = beyanname
                        .Element(ns + "Dokumanlar")?
                        .Elements(ns + "Dokuman")
                        .Where(d => Val(d, ns, "Kalem_no") == Val(kalem, ns, "Kalem_sira_no"))
                        .Select(d => new EvrimDokuman
                        {
                            KalemNo  = Int(d, ns, "Kalem_no") ?? 1,
                            Kod      = Val(d, ns, "Kod"),
                            Cevap    = Val(d, ns, "Dogrulama"),
                            Tarihi   = FormatTarih(Val(d, ns, "Belge_tarihi")),
                            Referans = Val(d, ns, "Referans"),
                        })
                        .ToList();

                    // Yurtiçi harcamalar (kalem seviyesi)
                    var yurtIciHarcamalari = new EvrimYurtIciHarcamalari
                    {
                        Ardiye          = Dec(kalem, ns, "Yurtici_Depolama") ?? 0,
                        TahmilTahliye   = Dec(kalem, ns, "Yurtici_Tahliye") ?? 0,
                        BankaMasraflari = Dec(kalem, ns, "Yurtici_Banka") ?? 0,
                        KkdfMatrah      = Dec(kalem, ns, "Yurtici_Kkdf") ?? 0,
                        KulturFonu      = Dec(kalem, ns, "Yurtici_Kultur") ?? 0,
                        Diger1          = Dec(kalem, ns, "Yurtici_Diger") ?? 0,
                        YurtIciCevre    = Dec(kalem, ns, "Yurtici_Cevre") ?? 0,
                        DigerAciklama   = Val(kalem, ns, "Yurtici_Diger_Aciklama"),
                    };

                    kalemler.Add(new EvrimDeclarationKalem
                    {
                        DetayNo                 = Int(kalem, ns, "Kalem_sira_no"),
                        GtipNo                  = Val(kalem, ns, "Gtip"),
                        MenseiUlke              = Val(kalem, ns, "Mensei_ulke"),
                        BrutAgirlik             = brut,
                        NetAgirlik              = net,
                        TamamlayiciOlcuBirimi   = Val(kalem, ns, "Tamamlayici_olcu_birimi"),
                        IstatistikiMiktar       = Dec(kalem, ns, "Istatistiki_miktar"),
                        UluslararasiAnlasma     = Val(kalem, ns, "Uluslararasi_anlasma"),
                        AlgilamaMiktari1        = Dec(kalem, ns, "Algilama_miktari_1"),
                        AlgilamaBirimi1         = Val(kalem, ns, "Algilama_birimi_1"),
                        AlgilamaMiktari2        = Dec(kalem, ns, "Algilama_miktari_2"),
                        AlgilamaBirimi2         = Val(kalem, ns, "Algilama_birimi_2"),
                        MuafKod                 = Val(kalem, ns, "Muafiyetler_1"),
                        MuafKod2                = Val(kalem, ns, "Muafiyetler_2"),
                        MuafKod3                = Val(kalem, ns, "Muafiyetler_3"),
                        MuafKod4                = Val(kalem, ns, "Muafiyetler_4"),
                        MuafKod5                = Val(kalem, ns, "Muafiyetler_5"),
                        EkKod                   = Val(kalem, ns, "Ek_kod"),
                        Ozellik                 = Val(kalem, ns, "Ozellik"),
                        KalemFiyati             = Dec(kalem, ns, "Fatura_miktari"),
                        Miktar                  = Dec(kalem, ns, "Miktar"),
                        MiktarBirimi            = Val(kalem, ns, "Miktar_birimi"),
                        Doviz                   = Val(kalem, ns, "Fatura_miktarinin_dovizi"),
                        NavlunMiktari           = Dec(kalem, ns, "Navlun_miktari"),
                        SigortaMiktari          = Dec(kalem, ns, "Sigorta_miktari"),
                        TicariTanim             = Val(kalem, ns, "Ticari_tanimi"),
                        EsyaTanimi1             = Val(kalem, ns, "Ticari_tanimi"),
                        Cinsi                   = Val(kalem, ns, "Cinsi"),
                        Adedi                   = Val(kalem, ns, "Adedi"),
                        Marka                   = Val(kalem, ns, "Marka"),
                        Numara                  = Val(kalem, ns, "Numara"),
                        IkincilIslem            = Val(kalem, ns, "Ikincil_islem"),
                        TesvikNo                = Val(kalem, ns, "Satir_no"),
                        KdvOrani                = Val(kalem, ns, "Kdv_orani"),
                        KullanilmisEsya         = Val(kalem, ns, "Kullanilmis_esya"),
                        KalemIslemNiteligi      = Val(kalem, ns, "Kalem_Islem_Niteligi"),
                        GirisCikisAmaci         = Val(kalem, ns, "Giris_Cikis_Amaci"),
                        GirisCikisAmaciAciklama = Val(kalem, ns, "Giris_Cikis_Amaci_Aciklama"),
                        EsyaGeriGelmeSebebi     = Val(kalem, ns, "EsyaGeriGelmeSebebi"),
                        EsyaGeriGelmeSebebiAciklama = Val(kalem, ns, "EsyaGeriGelmeSebebiAciklamasi"),
                        YurtDisiRoyalti         = Dec(kalem, ns, "YurtDisi_Royalti"),
                        YurtDisiRoyaltiDovizi   = Val(kalem, ns, "YurtDisi_Royalti_Dovizi"),
                        YurtDisiKomisyon        = Dec(kalem, ns, "YurtDisi_Komisyon"),
                        YurtDisiDepolama        = Dec(kalem, ns, "YurtDisi_Demuraj"),
                        FobTutar                = Dec(kalem, ns, "Fatura_miktari"),
                        FaturaNo                = parsedFaturaNo,
                        FaturaTarihi            = parsedFaturaTarihi,
                        YurtIciHarcamalari      = yurtIciHarcamalari,
                        OdemeSekilleri          = odemeSekilleri,
                        DokumanEdiBelgeler      = dokumanlar,
                    });
                }
            }

            // ── Vergiler (beyanname seviyesinde, Kalem_no ile gruplaniyor)
            var vergilerElement = beyanname.Element(ns + "Vergiler");
            if (vergilerElement != null)
            {
                var vergilerByKalem = vergilerElement.Elements(ns + "Vergi")
                    .GroupBy(v => Int(v, ns, "Kalem_no") ?? 1)
                    .ToDictionary(g => g.Key, g => g.Select((v, idx) => new EvrimVergi
                    {
                        SiraNo     = idx + 1,
                        Turu       = Val(v, ns, "Kod"),
                        Adi        = Val(v, ns, "Aciklama"),
                        Tutari     = Dec(v, ns, "Miktar") ?? 0,
                        Orani      = Dec(v, ns, "Oran") ?? 0,
                        OdemeSekli = Val(v, ns, "Odeme_sekli"),
                        Matrahi    = Dec(v, ns, "Vergi_matrahi") ?? 0,
                    }).ToList());

                foreach (var kalem in kalemler)
                {
                    var kalemNo = kalem.DetayNo ?? 1;
                    if (vergilerByKalem.TryGetValue(kalemNo, out var kalemVergiler))
                        kalem.Vergiler = kalemVergiler;
                }
            }

            // ── Firmalar
            var firmalar = firmaBilgi?
                .Elements(ns + "firma")
                .Select((f, i) => new EvrimFirma
                {
                    No       = i + 1,
                    Tip      = Val(f, ns, "Tip"),
                    Yfksiiks = Val(f, ns, "No"),
                    Adres    = Val(f, ns, "Cadde_s_no"),
                })
                .ToList();

            // ── Özet Beyanlar
            var ozetBeyanlar = beyanname
                .Element(ns + "Ozetbeyanlar")?
                .Elements(ns + "Ozetbeyan")
                .Select(ob => new EvrimOzetBeyan
                {
                    OzetbeyanNo           = Val(ob, ns, "Ozetbeyan_no"),
                    TasimaSenediNo        = ob.Element(ns + "ozbyacma_bilgi")
                                            ?.Element(ns + "tasimasenetleri")
                                            ?.Element(ns + "Tasima_senedi_no")?.Value?.Trim(),
                    BaskamRejim           = Val(ob, ns, "Baska_rejim"),
                    AmbarIci              = Val(ob, ns, "Ambar_ici"),
                    OzetbeyanIslemKapsami = Val(ob, ns, "Ozetbeyan_islem_kapsami"),
                    ToplamMiktar          = Dec(kalemlerElement?.Element(ns + "kalem"), ns, "Miktar"),
                    GtipNo                = Dec(kalemlerElement?.Element(ns + "kalem"), ns, "Gtip") != null
                                            ? Val(kalemlerElement!.Element(ns + "kalem")!, ns, "Gtip")
                                            : null,
                })
                .ToList();

            // ── Teminat
            var teminatEl = beyanname.Element(ns + "Teminat")?.Element(ns + "Teminat");

            // ── Yurtiçi harcamalar (beyanname seviyesi)
            var ilkKalemEl = kalemlerElement?.Element(ns + "kalem");
            var yurtIciBeyanname = ilkKalemEl != null ? new EvrimYurtIciHarcamalari
            {
                Ardiye          = Dec(ilkKalemEl, ns, "Yurtici_Depolama") ?? 0,
                TahmilTahliye   = Dec(ilkKalemEl, ns, "Yurtici_Tahliye") ?? 0,
                BankaMasraflari = Dec(ilkKalemEl, ns, "Yurtici_Banka") ?? 0,
                KkdfMatrah      = Dec(ilkKalemEl, ns, "Yurtici_Kkdf") ?? 0,
                KulturFonu      = Dec(ilkKalemEl, ns, "Yurtici_Kultur") ?? 0,
                Diger1          = Dec(ilkKalemEl, ns, "Yurtici_Diger") ?? 0,
                YurtIciCevre    = Dec(ilkKalemEl, ns, "Yurtici_Cevre") ?? 0,
                DigerAciklama   = Val(ilkKalemEl, ns, "Yurtici_Diger_Aciklama"),
            } : null;

            // ── KıymetBildirim
            var kiymetEl = beyanname.Element(ns + "KiymetBildirim")?.Element(ns + "Kiymet");
            EvrimKiymetBildirim? kiymetBildirim = null;
            if (kiymetEl != null)
            {
                var kiymetKalemler = kiymetEl
                    .Element(ns + "KiymetKalemler")?
                    .Elements(ns + "KiymetKalem")
                    .Select((kk, i) => new EvrimKiymetBildirimKalem
                    {
                        DetayNo                       = i + 1,
                        DolayliOdeme                  = Dec(kk, ns, "DolayliOdeme") ?? 0,
                        Komisyon                      = Dec(kk, ns, "Komisyon") ?? 0,
                        Tellaliye                     = Dec(kk, ns, "Tellaliye") ?? 0,
                        KapAmbalajBedeli              = Dec(kk, ns, "KapAmbalajBedeli") ?? 0,
                        IthalatKatilanMalzeme         = Dec(kk, ns, "IthalaKatilanMalzeme") ?? 0,
                        IthalatUretimAraclar          = Dec(kk, ns, "IthalaUretimAraclar") ?? 0,
                        IthalatUretimTuketimMalzemesi = Dec(kk, ns, "IthalaUretimTuketimMalzemesi") ?? 0,
                        PlanTaslak                    = Dec(kk, ns, "PlanTaslak") ?? 0,
                        RoyaltiLisans                 = Dec(kk, ns, "RoyaltiLisans") ?? 0,
                        DolayliIntikal                = Dec(kk, ns, "DolayliIntikal") ?? 0,
                        Nakliye                       = Dec(kk, ns, "Nakliye") ?? 0,
                        Sigorta                       = Dec(kk, ns, "Sigorta") ?? 0,
                        GirisSonrasiNakliye           = Dec(kk, ns, "GirisSonrasiNakliye") ?? 0,
                        TeknikYardim                  = Dec(kk, ns, "TeknikYardim") ?? 0,
                        DigerOdemeler                 = Dec(kk, ns, "DigerOdemeler") ?? 0,
                        DigerOdemelerNiteligi         = Val(kk, ns, "DigerOdemelerNiteligi"),
                        VergiHarcFon                  = Dec(kk, ns, "VergiHarcFon") ?? 0,
                    })
                    .ToList();

                kiymetBildirim = new EvrimKiymetBildirim
                {
                    AliciSaticiAyrintilar  = Val(kiymetEl, ns, "AliciSaticiAyrintilar"),
                    Edim                   = Val(kiymetEl, ns, "Edim"),
                    GumrukIdaresiKarari    = Val(kiymetEl, ns, "GumrukIdaresiKarari"),
                    Kisitlamalar           = Val(kiymetEl, ns, "Kisitlamalar"),
                    KisitlamalarAyrintilar = Val(kiymetEl, ns, "KisitlamalarAyrintilar"),
                    Munasebet              = Val(kiymetEl, ns, "AliciSatici"),
                    Royalti                = Val(kiymetEl, ns, "Royalti"),
                    RoyaltiKosullar        = Val(kiymetEl, ns, "RoyaltiKosullar"),
                    SaticiyaIntikal        = Val(kiymetEl, ns, "SaticiyaIntikal"),
                    SaticiyaIntikalKosullar = Val(kiymetEl, ns, "SaticiyaIntikalKosullar"),
                    SehirYer               = Val(kiymetEl, ns, "SehirYer"),
                    FaturaTarihiSayisi     = Val(kiymetEl, ns, "FaturaTarihiSayisi"),
                    SozlesmeTarihiSayisi   = Val(kiymetEl, ns, "SozlesmeTarihiSayisi"),
                    Taahutname             = Val(kiymetEl, ns, "Taahutname"),
                    KiymetBildirimKalemler = kiymetKalemler,
                };
            }

            // ── dosyaTarihi parse: Beyanname_tarihi → yyyy-MM-ddTHH:mm:ss
            var dosyaTarihiRaw = Val(beyanname, ns, "Beyanname_tarihi")
                ?? Val(beyanname, ns, "Dosya_tarihi");
            string? dosyaTarihi = null;
            if (!string.IsNullOrEmpty(dosyaTarihiRaw))
            {
                if (DateTime.TryParseExact(dosyaTarihiRaw, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtParsed))
                    dosyaTarihi = dtParsed.ToString("yyyy-MM-ddTHH:mm:ss");
                else
                    dosyaTarihi = dosyaTarihiRaw;
            }
            // Fallback: bugünün tarihi
            dosyaTarihi ??= DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            return new EvrimDeclarationRequest
            {
                // Kimlik
                // refId: Evrim'in Dosya Müşavir Referansı (max 5 karakter, sabit)
                // dosyaNo: Beyanname numarası (Evrim'de kayıt anahtarı)
                RefId              = null,
                Aktif              = true,
                DosyaNo            = Val(beyanname, ns, "Beyanname_no")
                                     ?? Val(beyanname, ns, "Referans_no"),
                BeyannameNo        = Val(beyanname, ns, "Beyanname_no")
                                     ?? Val(beyanname, ns, "Referans_no"),
                BeyannameTarihi    = dosyaTarihi,
                DosyaTarihi        = dosyaTarihi,
                Ihracat            = ihracat,
                DosyaTipi          = ihracat ? "H" : "T",
                RejimKodu          = rejimKodu,
                ReferansNo         = Val(beyanname, ns, "Referans_no"),
                IsTakipKodu        = Val(beyanname, ns, "Referans_no"),
                OlusturanKullanici = Val(beyanname, ns, "Kullanici_kodu"),

                // Müşteri
                MusteriVergi         = musteriVergi,
                MusteriUnvani        = musteriUnvani,
                MusteriAccountNumber = musteriVergi,

                // Beyan sahibi (sabit Orko değerleri)
                BeyanSahibiUnvan  = "ORKO GÜMRÜKLEME",
                BeyanSahibiVergiNo = Val(beyanname, ns, "Beyan_sahibi_vergi_no"),
                MusavirVergiNo    = Val(beyanname, ns, "Musavir_vergi_no"),
                KarnSahibiKodu    = "02",

                // Gümrük
                Gumruk              = Val(beyanname, ns, "GUMRUK"),
                BasitlestirilmisUsul = Val(beyanname, ns, "Basitlestirilmis_usul"),
                GirisGumrukIdaresi  = Val(beyanname, ns, "Giris_gumruk_idaresi"),
                VarisGumrukIdaresi  = Val(beyanname, ns, "Varis_gumruk_idaresi"),
                AntrepoKodu         = Val(beyanname, ns, "Antrepo_kodu"),
                EsyaninBulunduguYer = Val(beyanname, ns, "Esyanin_bulundugu_yer"),
                LimanKodu           = Val(beyanname, ns, "LimanKodu"),

                // Ülke
                TicaretUlkesi = Val(beyanname, ns, "Ticaret_ulkesi"),
                IlkUlke       = Val(beyanname, ns, "Cikis_ulkesi"),
                CikisUlkesi   = Val(beyanname, ns, "Cikis_ulkesi"),
                GidecegiUlke  = Val(beyanname, ns, "Gidecegi_ulke"),
                SevkUlkesi    = Val(beyanname, ns, "Gidecegi_sevk_ulkesi"),
                VarisUlkesi   = Val(beyanname, ns, "Gidecegi_sevk_ulkesi"),

                // Araç / Taşıma
                CikistakiAracinTipi    = Val(beyanname, ns, "Cikistaki_aracin_tipi"),
                CikistakiAracinKimligi = Val(beyanname, ns, "Cikistaki_aracin_kimligi"),
                CikistakiAracinUlkesi  = Val(beyanname, ns, "Cikistaki_aracin_ulkesi"),
                SinirdakiAracinTipi    = Val(beyanname, ns, "Sinirdaki_aracin_tipi"),
                SinirdakiAracinKimligi = Val(beyanname, ns, "Sinirdaki_aracin_kimligi"),
                SinirdakiAracinUlkesi  = Val(beyanname, ns, "Sinirdaki_aracin_ulkesi"),
                SinirdakiTasimaSekli   = Val(beyanname, ns, "Sinirdaki_tasima_sekli"),
                TeslimSekli            = Val(beyanname, ns, "Teslim_sekli"),
                TeslimYeri             = Val(beyanname, ns, "Teslim_yeri"),
                Konteyner              = konteyner,

                // Finansal
                ToplamFatura       = Dec(beyanname, ns, "Toplam_fatura"),
                ToplamFaturaDovizi = Val(beyanname, ns, "Toplam_fatura_dovizi"),
                ToplamNavlun       = Dec(beyanname, ns, "Toplam_navlun"),
                ToplamNavlunDovizi = Val(beyanname, ns, "Toplan_navlun_dovizi"),
                ToplamSigorta      = Dec(beyanname, ns, "Toplam_sigorta"),
                ToplamSigortaDovizi = Val(beyanname, ns, "Toplam_sigorta_dovizi"),
                ToplamYurtDisiHarcamalari       = Dec(beyanname, ns, "Toplam_yurt_disi_harcamalar"),
                ToplamYurtDisiHarcamalariDovizi = Val(beyanname, ns, "Toplam_yurt_disi_harcamalarin_dovizi"),
                ToplamYurtIciHarcamalari        = Dec(beyanname, ns, "Toplam_yurt_ici_harcamalar"),
                ToplamBrutAgirlik  = toplamBrut,
                ToplamNetAgirlik   = toplamNet,
                BrutAgirlik        = toplamBrut,
                NetAgirlik         = toplamNet,
                AliciSaticiIliskisi = Int(beyanname, ns, "Alici_satici_iliskisi"),

                // Kap / Yük
                KapAdedi           = Val(beyanname, ns, "Kap_adedi"),
                YukBelgeleriSayisi = Int(beyanname, ns, "Yuk_belgeleri_sayisi"),
                YukBelgesi         = Int(beyanname, ns, "Yuk_belgeleri_sayisi"),
                YuklemeBosaltmaYeri = Val(beyanname, ns, "Yukleme_bosaltma_yeri"),

                // Ödeme
                OdemeSekli = Val(beyanname, ns, "Odeme"),
                BankaKodu  = Val(beyanname, ns, "Banka_kodu"),

                // Notlar
                IsleminNiteligi = Val(beyanname, ns, "Islemin_niteligi"),
                Aciklamalar     = Val(beyanname, ns, "Aciklamalar"),
                SiparisTuru     = Val(ilkKalemEl, ns, "SiparisTuru"),
                TevTutar        = Dec(beyanname, ns, "Telafi_edici_vergi"),
                KkdfMatrah      = Dec(ilkKalemEl, ns, "Yurtici_Kkdf"),

                // Teminat
                TeminatSekli               = Val(teminatEl, ns, "Teminat_sekli"),
                TeminatOrani               = (int?)(Dec(teminatEl, ns, "Teminat_orani")),
                TeminatAciklama            = Val(teminatEl, ns, "Aciklama"),
                TeminatGlobalGarantiNo     = Val(teminatEl, ns, "Global_teminat_no"),
                TeminatDigerTutarReferansi = Val(teminatEl, ns, "Diger_tutar_referansi"),
                TeminatOdeme               = Dec(teminatEl, ns, "Nakdi_teminat_tutari")?.ToString(CultureInfo.InvariantCulture),

                // Birlik
                BirlikKayitNumara  = Val(beyanname, ns, "Birlik_kayit_numarasi"),
                BirlikKriptoNumara = Val(beyanname, ns, "Birlik_kripto_numarasi"),

                // Diğer
                AcentaBildirimNo      = Val(beyanname, ns, "AcentaSevkBildirimNo"),
                GumrukSaymanlikVergiNo = Val(beyanname, ns, "OdeSaymanlikBilgi"),
                FazlaMesaiTescilNo    = Val(beyanname, ns, "FazlaMesaiID"),

                // İlişkili objeler
                Firmalar          = firmalar,
                OzetBeyanlar      = ozetBeyanlar,
                YurtIciHarcamalari = yurtIciBeyanname,
                KiymetBildirim    = kiymetBildirim,

                Kalemler = kalemler,
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"GTS XML parse hatasi: {ex.Message}");
        }
    }

    // ── Yardımcı: "27/02/2026 GTSEXT1221" → ("GTSEXT1221", "2026-02-27")
    private static (string? faturaNo, string? faturaTarihi) ParseFaturaTarihiSayisi(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return (null, null);
        var parts = raw.Trim().Split(' ', 2);
        string? faturaNo = parts.Length > 1 ? parts[1].Trim() : null;
        string? faturaTarihi = null;
        if (DateTime.TryParseExact(parts[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            faturaTarihi = dt.ToString("yyyy-MM-dd");
        return (faturaNo, faturaTarihi);
    }

    // ── Yardımcı: "27/02/2026" → "2026-02-27" (Evrim yyyy-MM-dd bekler)
    private static string? FormatTarih(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        if (DateTime.TryParseExact(raw, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt.ToString("yyyy-MM-dd");
        return raw;
    }

    private static string? Val(XElement? parent, XNamespace ns, string name)
    {
        if (parent == null) return null;
        var v = parent.Element(ns + name)?.Value?.Trim();
        return string.IsNullOrEmpty(v) ? null : v;
    }

    private static decimal? Dec(XElement? parent, XNamespace ns, string name)
    {
        if (parent == null) return null;
        var v = parent.Element(ns + name)?.Value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return decimal.TryParse(v, CultureInfo.InvariantCulture, out var r) ? r : null;
    }

    private static int? Int(XElement? parent, XNamespace ns, string name)
    {
        if (parent == null) return null;
        var v = parent.Element(ns + name)?.Value?.Trim();
        if (string.IsNullOrEmpty(v)) return null;
        return int.TryParse(v, out var r) ? r : null;
    }
}