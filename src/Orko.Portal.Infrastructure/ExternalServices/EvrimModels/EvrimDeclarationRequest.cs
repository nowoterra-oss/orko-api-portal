using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim "Declaration" sema - POST /api/import ve POST /api/export icin kullanilir
/// </summary>
public class EvrimDeclarationRequest
{
    // --- Dosya Bilgileri ---
    [JsonPropertyName("refId")]
    public string? RefId { get; set; }

    [JsonPropertyName("dosyaTipi")]
    public string? DosyaTipi { get; set; }  // T: Ithalat, H: Ihracat, T-ANT: Antrepo

    [JsonPropertyName("dosyaNo")]
    public string? DosyaNo { get; set; }

    [JsonPropertyName("referansNo")]
    public string? ReferansNo { get; set; }  // Firma referansi (max 100)

    [JsonPropertyName("ihracat")]
    public bool? Ihracat { get; set; }

    [JsonPropertyName("olusturanKullanici")]
    public string? OlusturanKullanici { get; set; }

    [JsonPropertyName("dosyaTarihi")]
    public string? DosyaTarihi { get; set; }  // yyyy-MM-ddTHH:mm:ss

    // --- Musteri Bilgileri ---
    [JsonPropertyName("musteriVergi")]
    public string? MusteriVergi { get; set; }  // VKN (max 15)

    [JsonPropertyName("musteriUnvani")]
    public string? MusteriUnvani { get; set; }  // max 500

    [JsonPropertyName("musteriAccountNumber")]
    public string? MusteriAccountNumber { get; set; }

    // --- Beyanname Bilgileri ---
    [JsonPropertyName("rejimKodu")]
    public string? RejimKodu { get; set; }  // Orn: 4000

    [JsonPropertyName("gumruk")]
    public string? Gumruk { get; set; }  // Gumruk kodu Orn: "590300" (Evrim string bekler)

    [JsonPropertyName("basitlestirilmisUsul")]
    public string? BasitlestirilmisUsul { get; set; }

    // --- Ulke Bilgileri ---
    [JsonPropertyName("ilkUlke")]
    public string? IlkUlke { get; set; }

    [JsonPropertyName("gidecegiUlke")]
    public string? GidecegiUlke { get; set; }

    [JsonPropertyName("sevkUlkesi")]
    public string? SevkUlkesi { get; set; }

    [JsonPropertyName("ticaretUlkesi")]
    public string? TicaretUlkesi { get; set; }

    [JsonPropertyName("cikisUlkesi")]
    public string? CikisUlkesi { get; set; }

    [JsonPropertyName("varisUlkesi")]
    public string? VarisUlkesi { get; set; }

    // --- Tasima Bilgileri ---
    [JsonPropertyName("teslimSekli")]
    public string? TeslimSekli { get; set; }  // CIF, CFR vs (max 3)

    [JsonPropertyName("teslimYeri")]
    public string? TeslimYeri { get; set; }

    [JsonPropertyName("konteyner")]
    public string? Konteyner { get; set; }

    [JsonPropertyName("sinirdakiTasimaSekli")]
    public string? SinirdakiTasimaSekli { get; set; }

    [JsonPropertyName("sinirdakiAracinKimligi")]
    public string? SinirdakiAracinKimligi { get; set; }

    [JsonPropertyName("sinirdakiAracinTipi")]
    public string? SinirdakiAracinTipi { get; set; }

    [JsonPropertyName("sinirdakiAracinUlkesi")]
    public string? SinirdakiAracinUlkesi { get; set; }

    [JsonPropertyName("cikistakiAracinTipi")]
    public string? CikistakiAracinTipi { get; set; }

    [JsonPropertyName("cikistakiAracinKimligi")]
    public string? CikistakiAracinKimligi { get; set; }

    [JsonPropertyName("cikistakiAracinUlkesi")]
    public string? CikistakiAracinUlkesi { get; set; }

    // --- Fatura / Mali Bilgiler ---
    [JsonPropertyName("toplamFatura")]
    public decimal? ToplamFatura { get; set; }

    [JsonPropertyName("toplamFaturaDovizi")]
    public string? ToplamFaturaDovizi { get; set; }  // USD, EUR vs (max 3)

    [JsonPropertyName("toplamNavlun")]
    public decimal? ToplamNavlun { get; set; }

    [JsonPropertyName("toplamNavlunDovizi")]
    public string? ToplamNavlunDovizi { get; set; }

    [JsonPropertyName("toplamSigorta")]
    public decimal? ToplamSigorta { get; set; }

    [JsonPropertyName("toplamSigortaDovizi")]
    public string? ToplamSigortaDovizi { get; set; }

    [JsonPropertyName("toplamBrutAgirlik")]
    public decimal? ToplamBrutAgirlik { get; set; }

    [JsonPropertyName("toplamNetAgirlik")]
    public decimal? ToplamNetAgirlik { get; set; }

    // --- Kap / Yuk ---
    [JsonPropertyName("kapAdedi")]
    public string? KapAdedi { get; set; }

    [JsonPropertyName("yukBelgeleriSayisi")]
    public int? YukBelgeleriSayisi { get; set; }

    [JsonPropertyName("yuklemeBosaltmaYeri")]
    public string? YuklemeBosaltmaYeri { get; set; }

    // --- Diger ---
    [JsonPropertyName("odemeSekli")]
    public string? OdemeSekli { get; set; }

    [JsonPropertyName("bankaKodu")]
    public string? BankaKodu { get; set; }

    [JsonPropertyName("isleminNiteligi")]
    public string? IsleminNiteligi { get; set; }

    [JsonPropertyName("aciklamalar")]
    public string? Aciklamalar { get; set; }

    [JsonPropertyName("tahminiVarisTarihi")]
    public string? TahminiVarisTarihi { get; set; }

    [JsonPropertyName("beyanSahibiUnvan")]
    public string? BeyanSahibiUnvan { get; set; }

    [JsonPropertyName("beyanSahibiVergiNo")]
    public string? BeyanSahibiVergiNo { get; set; }

    [JsonPropertyName("musavirVergiNo")]
    public string? MusavirVergiNo { get; set; }

    // --- Kalemler ---
    [JsonPropertyName("kalemler")]
    public List<EvrimDeclarationKalem>? Kalemler { get; set; }
}

public class EvrimDeclarationKalem
{
    [JsonPropertyName("detayNo")]
    public int? DetayNo { get; set; }

    [JsonPropertyName("gtipNo")]
    public string? GtipNo { get; set; }  // max 16

    [JsonPropertyName("menseiUlke")]
    public string? MenseiUlke { get; set; }  // max 4

    [JsonPropertyName("brutAgirlik")]
    public decimal? BrutAgirlik { get; set; }

    [JsonPropertyName("netAgirlik")]
    public decimal? NetAgirlik { get; set; }

    [JsonPropertyName("istatistikiMiktar")]
    public decimal? IstatistikiMiktar { get; set; }

    [JsonPropertyName("kalemFiyati")]
    public decimal? KalemFiyati { get; set; }

    [JsonPropertyName("miktar")]
    public decimal? Miktar { get; set; }

    [JsonPropertyName("miktarBirimi")]
    public string? MiktarBirimi { get; set; }

    [JsonPropertyName("doviz")]
    public string? Doviz { get; set; }  // max 3

    [JsonPropertyName("ticariTanim")]
    public string? TicariTanim { get; set; }  // max 250

    [JsonPropertyName("faturaNo")]
    public string? FaturaNo { get; set; }

    [JsonPropertyName("faturaTarihi")]
    public string? FaturaTarihi { get; set; }  // yyyy-MM-dd

    [JsonPropertyName("cinsi")]
    public string? Cinsi { get; set; }

    [JsonPropertyName("adedi")]
    public string? Adedi { get; set; }

    [JsonPropertyName("navlunMiktari")]
    public decimal? NavlunMiktari { get; set; }

    [JsonPropertyName("sigortaMiktari")]
    public decimal? SigortaMiktari { get; set; }

    [JsonPropertyName("birimFiyat")]
    public decimal? BirimFiyat { get; set; }

    [JsonPropertyName("kdvOrani")]
    public string? KdvOrani { get; set; }

    [JsonPropertyName("kalemNotu")]
    public string? KalemNotu { get; set; }
}
