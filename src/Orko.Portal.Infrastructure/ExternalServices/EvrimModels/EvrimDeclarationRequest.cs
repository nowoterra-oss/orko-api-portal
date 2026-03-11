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

    [JsonPropertyName("beyannameNo")]
    public string? BeyannameNo { get; set; }

    [JsonPropertyName("beyannameTarihi")]
    public string? BeyannameTarihi { get; set; }  // yyyy-MM-ddTHH:mm:ss — Evrim zorunlu tutuyor

    [JsonPropertyName("referansNo")]
    public string? ReferansNo { get; set; }  // Firma referansi (max 100)

    [JsonPropertyName("ihracat")]
    public bool? Ihracat { get; set; }

    [JsonPropertyName("olusturanKullanici")]
    public string? OlusturanKullanici { get; set; }

    [JsonPropertyName("dosyaTarihi")]
    public string? DosyaTarihi { get; set; }  // yyyy-MM-ddTHH:mm:ss

    [JsonPropertyName("isTakipKodu")]
    public string? IsTakipKodu { get; set; }

    // --- Musteri Bilgileri ---
    [JsonPropertyName("musteriVergi")]
    public string? MusteriVergi { get; set; }  // VKN (max 15)

    [JsonPropertyName("musteriUnvani")]
    public string? MusteriUnvani { get; set; }  // max 500

    [JsonPropertyName("musteriAccountNumber")]
    public string? MusteriAccountNumber { get; set; }

    // --- Beyanname Bilgileri ---
    [JsonPropertyName("rejimKodu")]
    public string? RejimKodu { get; set; }  // Orn: 5100, 4000

    [JsonPropertyName("gumruk")]
    public string? Gumruk { get; set; }  // Gumruk kodu Orn: "590300" (Evrim string bekler)

    [JsonPropertyName("basitlestirilmisUsul")]
    public string? BasitlestirilmisUsul { get; set; }

    [JsonPropertyName("beyanSahibiUnvan")]
    public string? BeyanSahibiUnvan { get; set; }

    [JsonPropertyName("beyanSahibiVergiNo")]
    public string? BeyanSahibiVergiNo { get; set; }

    [JsonPropertyName("musavirVergiNo")]
    public string? MusavirVergiNo { get; set; }

    [JsonPropertyName("karneSahibi")]
    public int? KarneSahibi { get; set; }

    [JsonPropertyName("karnSahibiKodu")]
    public string? KarnSahibiKodu { get; set; }  // sabit: "02"

    [JsonPropertyName("karneSahibiAdi")]
    public string? KarneSahibiAdi { get; set; }

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
    public string? TeslimSekli { get; set; }  // CIF, CFR, FOB vs (max 3)

    [JsonPropertyName("teslimYeri")]
    public string? TeslimYeri { get; set; }

    [JsonPropertyName("konteyner")]
    public string? Konteyner { get; set; }  // "0" veya "1"

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

    [JsonPropertyName("toplamYurtDisiHarcamalari")]
    public decimal? ToplamYurtDisiHarcamalari { get; set; }

    [JsonPropertyName("toplamYurtDisiHarcamalariDovizi")]
    public string? ToplamYurtDisiHarcamalariDovizi { get; set; }

    [JsonPropertyName("toplamYurtIciHarcamalari")]
    public decimal? ToplamYurtIciHarcamalari { get; set; }

    [JsonPropertyName("toplamBrutAgirlik")]
    public decimal? ToplamBrutAgirlik { get; set; }

    [JsonPropertyName("toplamNetAgirlik")]
    public decimal? ToplamNetAgirlik { get; set; }

    [JsonPropertyName("brutAgirlik")]
    public decimal? BrutAgirlik { get; set; }

    [JsonPropertyName("netAgirlik")]
    public decimal? NetAgirlik { get; set; }

    // --- Kap / Yuk ---
    [JsonPropertyName("kapAdedi")]
    public string? KapAdedi { get; set; }

    [JsonPropertyName("yukBelgeleriSayisi")]
    public int? YukBelgeleriSayisi { get; set; }

    [JsonPropertyName("yukBelgesi")]
    public int? YukBelgesi { get; set; }

    [JsonPropertyName("yuklemeBosaltmaYeri")]
    public string? YuklemeBosaltmaYeri { get; set; }

    // --- Gumruk / Yer ---
    [JsonPropertyName("girisGumrukIdaresi")]
    public string? GirisGumrukIdaresi { get; set; }

    [JsonPropertyName("varisGumrukIdaresi")]
    public string? VarisGumrukIdaresi { get; set; }

    [JsonPropertyName("antrepoKodu")]
    public string? AntrepoKodu { get; set; }

    [JsonPropertyName("esyaninBulunduguYer")]
    public string? EsyaninBulunduguYer { get; set; }

    [JsonPropertyName("limanKodu")]
    public string? LimanKodu { get; set; }

    // --- Odeme / Banka ---
    [JsonPropertyName("odemeSekli")]
    public string? OdemeSekli { get; set; }

    [JsonPropertyName("bankaKodu")]
    public string? BankaKodu { get; set; }

    // --- Aciklama / Notlar ---
    [JsonPropertyName("isleminNiteligi")]
    public string? IsleminNiteligi { get; set; }

    [JsonPropertyName("aciklamalar")]
    public string? Aciklamalar { get; set; }

    [JsonPropertyName("siparisTuru")]
    public string? SiparisTuru { get; set; }

    [JsonPropertyName("tahminiVarisTarihi")]
    public string? TahminiVarisTarihi { get; set; }

    // --- Vergi / Teminat ---
    [JsonPropertyName("aliciSaticiIliskisi")]
    public int? AliciSaticiIliskisi { get; set; }

    [JsonPropertyName("tevTutar")]
    public decimal? TevTutar { get; set; }

    [JsonPropertyName("kkdfMatrah")]
    public decimal? KkdfMatrah { get; set; }

    [JsonPropertyName("teminatSekli")]
    public string? TeminatSekli { get; set; }

    [JsonPropertyName("teminatOrani")]
    public decimal? TeminatOrani { get; set; }

    [JsonPropertyName("teminatAciklama")]
    public string? TeminatAciklama { get; set; }

    [JsonPropertyName("teminatGlobalGarantiNo")]
    public string? TeminatGlobalGarantiNo { get; set; }

    [JsonPropertyName("teminatDigerTutarReferansi")]
    public string? TeminatDigerTutarReferansi { get; set; }

    [JsonPropertyName("teminatOdeme")]
    public decimal? TeminatOdeme { get; set; }

    // --- Birlik ---
    [JsonPropertyName("birlikKayitNumara")]
    public string? BirlikKayitNumara { get; set; }

    [JsonPropertyName("birlikKriptoNumara")]
    public string? BirlikKriptoNumara { get; set; }

    [JsonPropertyName("birlikReferans")]
    public string? BirlikReferans { get; set; }

    [JsonPropertyName("birlikKodu")]
    public string? BirlikKodu { get; set; }

    [JsonPropertyName("birlikAdi")]
    public string? BirlikAdi { get; set; }

    // --- Diger ---
    [JsonPropertyName("acentaBildirimNo")]
    public string? AcentaBildirimNo { get; set; }

    [JsonPropertyName("gumrukSaymanlikVergiNo")]
    public string? GumrukSaymanlikVergiNo { get; set; }

    [JsonPropertyName("fazlaMesaiTescilNo")]
    public string? FazlaMesaiTescilNo { get; set; }

    // --- Iliskili Objeler ---
    [JsonPropertyName("firmalar")]
    public List<EvrimFirma>? Firmalar { get; set; }

    [JsonPropertyName("ozetBeyanlar")]
    public List<EvrimOzetBeyan>? OzetBeyanlar { get; set; }

    [JsonPropertyName("yurtIciHarcamalari")]
    public EvrimYurtIciHarcamalari? YurtIciHarcamalari { get; set; }

    [JsonPropertyName("kiymetBildirim")]
    public EvrimKiymetBildirim? KiymetBildirim { get; set; }

    [JsonPropertyName("firmaEkBilgiler")]
    public object? FirmaEkBilgiler { get; set; }

    [JsonPropertyName("isTakipler")]
    public List<object>? IsTakipler { get; set; }

    // --- Kalemler ---
    [JsonPropertyName("kalemler")]
    public List<EvrimDeclarationKalem>? Kalemler { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  KALEM
// ════════════════════════════════════════════════════════════════
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

    [JsonPropertyName("tamamlayiciOlcuBirimi")]
    public string? TamamlayiciOlcuBirimi { get; set; }

    [JsonPropertyName("uluslararasiAnlasma")]
    public string? UluslararasiAnlasma { get; set; }

    [JsonPropertyName("algilamaMiktari1")]
    public decimal? AlgilamaMiktari1 { get; set; }

    [JsonPropertyName("algilamaBirimi1")]
    public string? AlgilamaBirimi1 { get; set; }

    [JsonPropertyName("algilamaMiktari2")]
    public decimal? AlgilamaMiktari2 { get; set; }

    [JsonPropertyName("algilamaBirimi2")]
    public string? AlgilamaBirimi2 { get; set; }

    [JsonPropertyName("muafKod")]
    public string? MuafKod { get; set; }

    [JsonPropertyName("muafKod2")]
    public string? MuafKod2 { get; set; }

    [JsonPropertyName("muafKod3")]
    public string? MuafKod3 { get; set; }

    [JsonPropertyName("muafKod4")]
    public string? MuafKod4 { get; set; }

    [JsonPropertyName("muafKod5")]
    public string? MuafKod5 { get; set; }

    [JsonPropertyName("ekKod")]
    public string? EkKod { get; set; }

    [JsonPropertyName("ozellik")]
    public string? Ozellik { get; set; }

    [JsonPropertyName("kalemFiyati")]
    public decimal? KalemFiyati { get; set; }

    [JsonPropertyName("miktar")]
    public decimal? Miktar { get; set; }

    [JsonPropertyName("miktarBirimi")]
    public string? MiktarBirimi { get; set; }

    [JsonPropertyName("doviz")]
    public string? Doviz { get; set; }  // max 3

    [JsonPropertyName("navlunMiktari")]
    public decimal? NavlunMiktari { get; set; }

    [JsonPropertyName("sigortaMiktari")]
    public decimal? SigortaMiktari { get; set; }

    [JsonPropertyName("ticariTanim")]
    public string? TicariTanim { get; set; }  // max 250

    [JsonPropertyName("esyaTanimi1")]
    public string? EsyaTanimi1 { get; set; }

    [JsonPropertyName("esyaTanimi2")]
    public string? EsyaTanimi2 { get; set; }

    [JsonPropertyName("cinsi")]
    public string? Cinsi { get; set; }

    [JsonPropertyName("adedi")]
    public string? Adedi { get; set; }

    [JsonPropertyName("marka")]
    public string? Marka { get; set; }

    [JsonPropertyName("numara")]
    public string? Numara { get; set; }

    [JsonPropertyName("ikincilIslem")]
    public string? IkincilIslem { get; set; }

    [JsonPropertyName("tesvikNo")]
    public string? TesvikNo { get; set; }  // DİİB satır no

    [JsonPropertyName("faturaNo")]
    public string? FaturaNo { get; set; }

    [JsonPropertyName("faturaTarihi")]
    public string? FaturaTarihi { get; set; }  // yyyy-MM-dd

    [JsonPropertyName("birimFiyat")]
    public decimal? BirimFiyat { get; set; }

    [JsonPropertyName("kdvOrani")]
    public string? KdvOrani { get; set; }

    [JsonPropertyName("kalemNotu")]
    public string? KalemNotu { get; set; }

    [JsonPropertyName("kullanilmisEsya")]
    public string? KullanilmisEsya { get; set; }

    [JsonPropertyName("kalemIslemNiteligi")]
    public string? KalemIslemNiteligi { get; set; }

    [JsonPropertyName("girisCikisAmaci")]
    public string? GirisCikisAmaci { get; set; }

    [JsonPropertyName("girisCikisAmaciAciklama")]
    public string? GirisCikisAmaciAciklama { get; set; }

    [JsonPropertyName("esyaGeriGelmeSebebi")]
    public string? EsyaGeriGelmeSebebi { get; set; }

    [JsonPropertyName("esyaGeriGelmeSebebiAciklama")]
    public string? EsyaGeriGelmeSebebiAciklama { get; set; }

    [JsonPropertyName("fobTutar")]
    public decimal? FobTutar { get; set; }

    [JsonPropertyName("yurtDisiRoyalti")]
    public decimal? YurtDisiRoyalti { get; set; }

    [JsonPropertyName("yurtDisiRoyaltiDovizi")]
    public string? YurtDisiRoyaltiDovizi { get; set; }

    [JsonPropertyName("yurtDisiKomisyon")]
    public decimal? YurtDisiKomisyon { get; set; }

    [JsonPropertyName("yurtDisiDepolama")]
    public decimal? YurtDisiDepolama { get; set; }

    [JsonPropertyName("yurtIciHarcamalari")]
    public EvrimYurtIciHarcamalari? YurtIciHarcamalari { get; set; }

    [JsonPropertyName("odemeSekilleri")]
    public List<EvrimOdemeSekli>? OdemeSekilleri { get; set; }

    [JsonPropertyName("vergiler")]
    public List<EvrimVergi>? Vergiler { get; set; }

    [JsonPropertyName("dokumanEdiBelgeler")]
    public List<EvrimDokuman>? DokumanEdiBelgeler { get; set; }

    [JsonPropertyName("malCinsleri")]
    public List<object>? MalCinsleri { get; set; }

    [JsonPropertyName("konteynerBilgi")]
    public List<object>? KonteynerBilgi { get; set; }

    [JsonPropertyName("tcgbKapatmaBilgileri")]
    public List<object>? TcgbKapatmaBilgileri { get; set; }

    [JsonPropertyName("firmaEkBilgiler")]
    public object? FirmaEkBilgiler { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  YARDIMCI SINIFLAR
// ════════════════════════════════════════════════════════════════

public class EvrimFirma
{
    [JsonPropertyName("no")]
    public int No { get; set; }

    [JsonPropertyName("tip")]
    public string? Tip { get; set; }  // Gonderici, Alici vb.

    [JsonPropertyName("accountNo")]
    public string? AccountNo { get; set; }

    [JsonPropertyName("vkn")]
    public string? Vkn { get; set; }

    [JsonPropertyName("yfksiiks")]
    public string? Yfksiiks { get; set; }  // YFKS/IİKS numarası

    [JsonPropertyName("adi")]
    public string? Adi { get; set; }

    [JsonPropertyName("ulkeNo")]
    public string? UlkeNo { get; set; }

    [JsonPropertyName("adres")]
    public string? Adres { get; set; }

    [JsonPropertyName("maliSorumlu")]
    public string? MaliSorumlu { get; set; }

    [JsonPropertyName("maliSorumluVkn")]
    public string? MaliSorumluVkn { get; set; }
}

public class EvrimOzetBeyan
{
    [JsonPropertyName("ozetbeyanNo")]
    public string? OzetbeyanNo { get; set; }

    [JsonPropertyName("ozetbeyanTarihi")]
    public string? OzetbeyanTarihi { get; set; }  // cevap XML'den

    [JsonPropertyName("tasimaSenediNo")]
    public string? TasimaSenediNo { get; set; }

    [JsonPropertyName("baskamRejim")]
    public string? BaskamRejim { get; set; }

    [JsonPropertyName("ambarIci")]
    public string? AmbarIci { get; set; }

    [JsonPropertyName("ozetbeyanIslemKapsami")]
    public string? OzetbeyanIslemKapsami { get; set; }

    [JsonPropertyName("toplamMiktar")]
    public decimal? ToplamMiktar { get; set; }

    [JsonPropertyName("gtipNo")]
    public string? GtipNo { get; set; }
}

public class EvrimYurtIciHarcamalari
{
    [JsonPropertyName("ardiye")]
    public decimal Ardiye { get; set; }

    [JsonPropertyName("tahmilTahliye")]
    public decimal TahmilTahliye { get; set; }

    [JsonPropertyName("bankaMasraflari")]
    public decimal BankaMasraflari { get; set; }

    [JsonPropertyName("kkdfMatrah")]
    public decimal KkdfMatrah { get; set; }

    [JsonPropertyName("kulturFonu")]
    public decimal KulturFonu { get; set; }

    [JsonPropertyName("diger1")]
    public decimal Diger1 { get; set; }

    [JsonPropertyName("diger2")]
    public decimal Diger2 { get; set; }

    [JsonPropertyName("yurtIciCevre")]
    public decimal YurtIciCevre { get; set; }

    [JsonPropertyName("digerAciklama")]
    public string? DigerAciklama { get; set; }
}

public class EvrimOdemeSekli
{
    [JsonPropertyName("kodu")]
    public int? Kodu { get; set; }

    [JsonPropertyName("tutari")]
    public decimal? Tutari { get; set; }

    [JsonPropertyName("tranferBildirimNo")]
    public string? TranferBildirimNo { get; set; }

    [JsonPropertyName("tranferBildirimTarihi")]
    public string? TranferBildirimTarihi { get; set; }
}

public class EvrimVergi
{
    [JsonPropertyName("siraNo")]
    public int SiraNo { get; set; }

    [JsonPropertyName("turu")]
    public string? Turu { get; set; }

    [JsonPropertyName("matrahi")]
    public decimal Matrahi { get; set; }

    [JsonPropertyName("orani")]
    public decimal Orani { get; set; }

    [JsonPropertyName("tutari")]
    public decimal Tutari { get; set; }

    [JsonPropertyName("odemeSekli")]
    public string? OdemeSekli { get; set; }

    [JsonPropertyName("vy")]
    public string? Vy { get; set; }

    [JsonPropertyName("adi")]
    public string? Adi { get; set; }
}

public class EvrimDokuman
{
    [JsonPropertyName("kalemNo")]
    public int KalemNo { get; set; }

    [JsonPropertyName("kod")]
    public string? Kod { get; set; }

    [JsonPropertyName("tip")]
    public string? Tip { get; set; }

    [JsonPropertyName("cevap")]
    public string? Cevap { get; set; }  // V: Var

    [JsonPropertyName("tarihi")]
    public string? Tarihi { get; set; }  // yyyy-MM-dd

    [JsonPropertyName("referans")]
    public string? Referans { get; set; }

    [JsonPropertyName("tescilli")]
    public string? Tescilli { get; set; }
}

public class EvrimKiymetBildirim
{
    [JsonPropertyName("aliciSaticiAyrintilar")]
    public string? AliciSaticiAyrintilar { get; set; }

    [JsonPropertyName("edim")]
    public string? Edim { get; set; }

    [JsonPropertyName("gumrukIdaresiKarari")]
    public string? GumrukIdaresiKarari { get; set; }

    [JsonPropertyName("kisitlamalar")]
    public string? Kisitlamalar { get; set; }

    [JsonPropertyName("kisitlamalarAyrintilar")]
    public string? KisitlamalarAyrintilar { get; set; }

    [JsonPropertyName("munasebet")]
    public string? Munasebet { get; set; }

    [JsonPropertyName("royalti")]
    public string? Royalti { get; set; }

    [JsonPropertyName("royaltiKosullar")]
    public string? RoyaltiKosullar { get; set; }

    [JsonPropertyName("saticiyaIntikal")]
    public string? SaticiyaIntikal { get; set; }

    [JsonPropertyName("saticiyaIntikalKosullar")]
    public string? SaticiyaIntikalKosullar { get; set; }

    [JsonPropertyName("sehirYer")]
    public string? SehirYer { get; set; }

    [JsonPropertyName("sozlesmeTarihiSayisi")]
    public string? SozlesmeTarihiSayisi { get; set; }

    [JsonPropertyName("faturaTarihiSayisi")]
    public string? FaturaTarihiSayisi { get; set; }

    [JsonPropertyName("taahutname")]
    public string? Taahutname { get; set; }

    [JsonPropertyName("kiymetBildirimKalemler")]
    public List<EvrimKiymetBildirimKalem>? KiymetBildirimKalemler { get; set; }
}

public class EvrimKiymetBildirimKalem
{
    [JsonPropertyName("detayNo")]
    public int DetayNo { get; set; }

    [JsonPropertyName("dolayliOdeme")]
    public decimal DolayliOdeme { get; set; }

    [JsonPropertyName("komisyon")]
    public decimal Komisyon { get; set; }

    [JsonPropertyName("tellaliye")]
    public decimal Tellaliye { get; set; }

    [JsonPropertyName("kapAmbalajBedeli")]
    public decimal KapAmbalajBedeli { get; set; }

    [JsonPropertyName("ithalatKatilanMalzeme")]
    public decimal IthalatKatilanMalzeme { get; set; }

    [JsonPropertyName("IthalatUretimAraclar")]
    public decimal IthalatUretimAraclar { get; set; }

    [JsonPropertyName("IthalatUretimTuketimMalzemesi")]
    public decimal IthalatUretimTuketimMalzemesi { get; set; }

    [JsonPropertyName("planTaslak")]
    public decimal PlanTaslak { get; set; }

    [JsonPropertyName("royaltiLisans")]
    public decimal RoyaltiLisans { get; set; }

    [JsonPropertyName("dolayliIntikal")]
    public decimal DolayliIntikal { get; set; }

    [JsonPropertyName("nakliye")]
    public decimal Nakliye { get; set; }

    [JsonPropertyName("sigorta")]
    public decimal Sigorta { get; set; }

    [JsonPropertyName("girisSonrasiNakliye")]
    public decimal GirisSonrasiNakliye { get; set; }

    [JsonPropertyName("teknikYardim")]
    public decimal TeknikYardim { get; set; }

    [JsonPropertyName("digerOdemeler")]
    public decimal DigerOdemeler { get; set; }

    [JsonPropertyName("digerOdemelerNiteligi")]
    public string? DigerOdemelerNiteligi { get; set; }

    [JsonPropertyName("vergiHarcFon")]
    public decimal VergiHarcFon { get; set; }
}