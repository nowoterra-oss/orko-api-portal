using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace OrkoPortal.Infrastructure.Mapping
{
    // ════════════════════════════════════════════════════════════════
    //  ANA BEYANNAME DTO
    // ════════════════════════════════════════════════════════════════
    public class EvrimBeyannameDto
    {
        // ── Kimlik / Dosya
        [JsonPropertyName("refId")] public string? RefId { get; set; }
        [JsonPropertyName("dosyaTipi")] public string? DosyaTipi { get; set; }
        [JsonPropertyName("dosyaNo")] public string? DosyaNo { get; set; }
        [JsonPropertyName("beyannameNo")] public string? BeyannameNo { get; set; }          // cevap XML
        [JsonPropertyName("beyannameTarihi")] public string? BeyannameTarihi { get; set; }      // cevap XML
        [JsonPropertyName("referansNo")] public string? ReferansNo { get; set; }
        [JsonPropertyName("musteriVergi")] public string? MusteriVergi { get; set; }
        [JsonPropertyName("musteriUnvani")] public string? MusteriUnvani { get; set; }        // elle doldurulacak
        [JsonPropertyName("musteriAccountNumber")] public string? MusteriAccountNumber { get; set; }
        [JsonPropertyName("olusturanKullanici")] public string? OlusturanKullanici { get; set; }
        [JsonPropertyName("dosyaTarihi")] public string? DosyaTarihi { get; set; }          // cevap XML
        [JsonPropertyName("ihracat")] public bool Ihracat { get; set; }                 // EX/IM prefix'ten
        [JsonPropertyName("rejimKodu")] public string? RejimKodu { get; set; }
        [JsonPropertyName("gumruk")] public int? Gumruk { get; set; }
        [JsonPropertyName("basitlestirilmisUsul")] public string? BasitlestirilmisUsul { get; set; }
        [JsonPropertyName("yukBelgeleriSayisi")] public int? YukBelgeleriSayisi { get; set; }
        [JsonPropertyName("yukBelgesi")] public int? YukBelgesi { get; set; }

        // ── Ülke / Rota
        [JsonPropertyName("ilkUlke")] public string? IlkUlke { get; set; }
        [JsonPropertyName("gidecegiUlke")] public string? GidecegiUlke { get; set; }
        [JsonPropertyName("sevkUlkesi")] public string? SevkUlkesi { get; set; }
        [JsonPropertyName("ticaretUlkesi")] public string? TicaretUlkesi { get; set; }
        [JsonPropertyName("cikisUlkesi")] public string? CikisUlkesi { get; set; }
        [JsonPropertyName("varisUlkesi")] public string? VarisUlkesi { get; set; }

        // ── Araç
        [JsonPropertyName("cikistakiAracinTipi")] public string? CikistakiAracinTipi { get; set; }
        [JsonPropertyName("cikistakiAracinUlkesi")] public string? CikistakiAracinUlkesi { get; set; }
        [JsonPropertyName("cikistakiAracinKimligi")] public string? CikistakiAracinKimligi { get; set; }
        [JsonPropertyName("sinirdakiAracinTipi")] public string? SinirdakiAracinTipi { get; set; }
        [JsonPropertyName("sinirdakiAracinKimligi")] public string? SinirdakiAracinKimligi { get; set; }
        [JsonPropertyName("sinirdakiAracinUlkesi")] public string? SinirdakiAracinUlkesi { get; set; }
        [JsonPropertyName("sinirdakiTasimaSekli")] public string? SinirdakiTasimaSekli { get; set; }

        // ── Finansal
        [JsonPropertyName("teslimSekli")] public string? TeslimSekli { get; set; }
        [JsonPropertyName("teslimYeri")] public string? TeslimYeri { get; set; }
        [JsonPropertyName("konteyner")] public string? Konteyner { get; set; }
        [JsonPropertyName("toplamFatura")] public decimal ToplamFatura { get; set; }
        [JsonPropertyName("toplamFaturaDovizi")] public string? ToplamFaturaDovizi { get; set; }
        [JsonPropertyName("toplamNavlun")] public decimal ToplamNavlun { get; set; }
        [JsonPropertyName("toplamNavlunDovizi")] public string? ToplamNavlunDovizi { get; set; }
        [JsonPropertyName("aliciSaticiIliskisi")] public int? AliciSaticiIliskisi { get; set; }
        [JsonPropertyName("toplamSigorta")] public decimal ToplamSigorta { get; set; }
        [JsonPropertyName("toplamSigortaDovizi")] public string? ToplamSigortaDovizi { get; set; }
        [JsonPropertyName("toplamYurtDisiHarcamalari")] public decimal ToplamYurtDisiHarcamalari { get; set; }
        [JsonPropertyName("toplamYurtDisiHarcamalariDovizi")] public string? ToplamYurtDisiHarcamalariDovizi { get; set; }
        [JsonPropertyName("toplamYurtDisiHarcamalariAciklama")] public string? ToplamYurtDisiHarcamalariAciklama { get; set; }
        [JsonPropertyName("toplamYurtIciHarcamalari")] public decimal ToplamYurtIciHarcamalari { get; set; }
        [JsonPropertyName("yurtDisiRoyalti")] public decimal YurtDisiRoyalti { get; set; }
        [JsonPropertyName("yurtDisiRoyaltiDovizi")] public string? YurtDisiRoyaltiDovizi { get; set; }
        [JsonPropertyName("yurtDisiBanka")] public decimal YurtDisiBanka { get; set; }
        [JsonPropertyName("yurtDisiKomisyon")] public decimal YurtDisiKomisyon { get; set; }
        [JsonPropertyName("yurtDisiDepolama")] public decimal YurtDisiDepolama { get; set; }
        [JsonPropertyName("odemeSekli")] public string? OdemeSekli { get; set; }
        [JsonPropertyName("bankaKodu")] public string? BankaKodu { get; set; }

        // ── Yer / Gümrük
        [JsonPropertyName("varisGumrukIdaresi")] public int? VarisGumrukIdaresi { get; set; }
        [JsonPropertyName("antrepoKodu")] public string? AntrepoKodu { get; set; }
        [JsonPropertyName("esyaninBulunduguYer")] public string? EsyaninBulunduguYer { get; set; }
        [JsonPropertyName("limanKodu")] public string? LimanKodu { get; set; }
        [JsonPropertyName("girisGumrukIdaresi")] public string? GirisGumrukIdaresi { get; set; }
        [JsonPropertyName("yuklemeBosaltmaYeri")] public string? YuklemeBosaltmaYeri { get; set; }
        [JsonPropertyName("kapAdedi")] public string? KapAdedi { get; set; }

        // ── Açıklamalar / Notlar
        [JsonPropertyName("isleminNiteligi")] public string? IsleminNiteligi { get; set; }
        [JsonPropertyName("aciklamalar")] public string? Aciklamalar { get; set; }
        [JsonPropertyName("isTakipKodu")] public string? IsTakipKodu { get; set; }
        [JsonPropertyName("siparisTuru")] public string? SiparisTuru { get; set; }
        [JsonPropertyName("eTicaret")] public string? ETicaret { get; set; }              // cevap XML

        // ── Vergi / Teminat
        [JsonPropertyName("tevTutar")] public decimal TevTutar { get; set; }
        [JsonPropertyName("kkdfMatrah")] public decimal KkdfMatrah { get; set; }
        [JsonPropertyName("toplamVergi")] public decimal ToplamVergi { get; set; }           // cevap XML
        [JsonPropertyName("teminatDeger")] public decimal TeminatDeger { get; set; }
        [JsonPropertyName("teminatSekli")] public string? TeminatSekli { get; set; }
        [JsonPropertyName("teminatDigerTutarReferansi")] public string? TeminatDigerTutarReferansi { get; set; }
        [JsonPropertyName("teminatGlobalGarantiNo")] public string? TeminatGlobalGarantiNo { get; set; }
        [JsonPropertyName("teminatOrani")] public decimal TeminatOrani { get; set; }
        [JsonPropertyName("teminatAciklama")] public string? TeminatAciklama { get; set; }
        [JsonPropertyName("teminatOdeme")] public decimal TeminatOdeme { get; set; }

        // ── Ağırlık
        [JsonPropertyName("toplamBrutAgirlik")] public decimal ToplamBrutAgirlik { get; set; }
        [JsonPropertyName("toplamNetAgirlik")] public decimal ToplamNetAgirlik { get; set; }
        [JsonPropertyName("brutAgirlik")] public decimal BrutAgirlik { get; set; }
        [JsonPropertyName("netAgirlik")] public decimal NetAgirlik { get; set; }

        // ── Karne / Beyan Sahibi (Sabit değerler)
        [JsonPropertyName("karneSahibi")] public int? KarneSahibi { get; set; }
        [JsonPropertyName("karnSahibiKodu")] public string? KarnSahibiKodu { get; set; }        // sabit: "02"
        [JsonPropertyName("karneSahibiAdi")] public string? KarneSahibiAdi { get; set; }        // sabit
        [JsonPropertyName("beyanSahibiUnvan")] public string? BeyanSahibiUnvan { get; set; }      // sabit: "ORKO GÜMRÜKLEME"
        [JsonPropertyName("beyanSahibiVergiNo")] public string? BeyanSahibiVergiNo { get; set; }
        [JsonPropertyName("musavirVergiNo")] public string? MusavirVergiNo { get; set; }
        [JsonPropertyName("muayeneMemuru")] public string? MuayeneMemuru { get; set; }         // cevap XML

        // ── Birlik
        [JsonPropertyName("birlikKayitNumara")] public string? BirlikKayitNumarasi { get; set; }
        [JsonPropertyName("birlikKriptoNumara")] public string? BirlikKriptoNumarasi { get; set; }
        [JsonPropertyName("birlikReferans")] public string? BirlikReferans { get; set; }
        [JsonPropertyName("birlikKodu")] public string? BirlikKodu { get; set; }
        [JsonPropertyName("birlikAdi")] public string? BirlikAdi { get; set; }

        // ── Diğer
        [JsonPropertyName("acentaBildirimNo")] public string? AcentaBildirimNo { get; set; }
        [JsonPropertyName("gumrukSaymanlikVergiNo")] public string? GumrukSaymanlikVergiNo { get; set; }
        [JsonPropertyName("fazlaMesaiTescilNo")] public string? FazlaMesaiTescilNo { get; set; }

        // ── İlişkili objeler
        [JsonPropertyName("yurtIciHarcamalari")] public YurtIciHarcamalariDto? YurtIciHarcamalari { get; set; }
        [JsonPropertyName("kiymetBildirim")] public KiymetBildirimDto? KiymetBildirim { get; set; }
        [JsonPropertyName("firmalar")] public List<FirmaDto> Firmalar { get; set; } = new();
        [JsonPropertyName("ozetBeyanlar")] public List<OzetBeyanDto> OzetBeyanlar { get; set; } = new();
        [JsonPropertyName("kalemler")] public List<KalemDto> Kalemler { get; set; } = new();
        [JsonPropertyName("firmaEkBilgiler")] public object? FirmaEkBilgiler { get; set; } = new { };
        [JsonPropertyName("isTakipler")] public List<object> IsTakipler { get; set; } = new();
    }

    // ════════════════════════════════════════════════════════════════
    //  KALEM DTO
    // ════════════════════════════════════════════════════════════════
    public class KalemDto
    {
        [JsonPropertyName("gtipNo")] public string? GtipNo { get; set; }
        [JsonPropertyName("menseiUlke")] public string? MenseiUlke { get; set; }
        [JsonPropertyName("brutAgirlik")] public decimal BrutAgirlik { get; set; }
        [JsonPropertyName("netAgirlik")] public decimal NetAgirlik { get; set; }
        [JsonPropertyName("tamamlayiciOlcuBirimi")] public string? TamamlayiciOlcuBirimi { get; set; }
        [JsonPropertyName("istatistikiMiktar")] public decimal IstatistikiMiktar { get; set; }
        [JsonPropertyName("uluslararasiAnlasma")] public string? UluslararasiAnlasma { get; set; }
        [JsonPropertyName("algilamaMiktari1")] public decimal AlgilamaMiktari1 { get; set; }
        [JsonPropertyName("algilamaBirimi1")] public string? AlgilamaBirimi1 { get; set; }
        [JsonPropertyName("algilamaMiktari2")] public decimal AlgilamaMiktari2 { get; set; }
        [JsonPropertyName("algilamaBirimi2")] public string? AlgilamaBirimi2 { get; set; }
        [JsonPropertyName("algilamaMiktari3")] public decimal AlgilamaMiktari3 { get; set; }
        [JsonPropertyName("algilamaBirimi3")] public string? AlgilamaBirimi3 { get; set; }
        [JsonPropertyName("muafKod")] public string? MuafKod { get; set; }
        [JsonPropertyName("muafKod2")] public string? MuafKod2 { get; set; }
        [JsonPropertyName("muafKod3")] public string? MuafKod3 { get; set; }
        [JsonPropertyName("muafKod4")] public string? MuafKod4 { get; set; }
        [JsonPropertyName("muafKod5")] public string? MuafKod5 { get; set; }
        [JsonPropertyName("ekKod")] public string? EkKod { get; set; }
        [JsonPropertyName("ozellik")] public string? Ozellik { get; set; }
        [JsonPropertyName("kalemFiyati")] public decimal KalemFiyati { get; set; }
        [JsonPropertyName("navlunMiktari")] public decimal NavlunMiktari { get; set; }
        [JsonPropertyName("sigortaMiktari")] public decimal SigortaMiktari { get; set; }
        [JsonPropertyName("ydHesaplama")] public string? YdHesaplama { get; set; }
        [JsonPropertyName("ticariTanim")] public string? TicariTanim { get; set; }
        [JsonPropertyName("esyaTanimi1")] public string? EsyaTanimi1 { get; set; }
        [JsonPropertyName("esyaTanimi2")] public string? EsyaTanimi2 { get; set; }
        [JsonPropertyName("esyaTanimi3")] public string? EsyaTanimi3 { get; set; }
        [JsonPropertyName("cinsi")] public string? Cinsi { get; set; }
        [JsonPropertyName("adedi")] public string? Adedi { get; set; }
        [JsonPropertyName("miktarBirimi")] public string? MiktarBirimi { get; set; }
        [JsonPropertyName("ikincilIslem")] public string? IkincilIslem { get; set; }
        [JsonPropertyName("tesvikNo")] public string? TesvikNo { get; set; }
        [JsonPropertyName("miktar")] public decimal Miktar { get; set; }
        [JsonPropertyName("kdvOrani")] public string? KdvOrani { get; set; }           // cevap XML
        [JsonPropertyName("kullanilmisEsya")] public string? KullanilmisEsya { get; set; }
        [JsonPropertyName("marka")] public string? Marka { get; set; }
        [JsonPropertyName("numara")] public string? Numara { get; set; }
        [JsonPropertyName("doviz")] public string? Doviz { get; set; }
        [JsonPropertyName("kalemIslemNiteligi")] public string? KalemIslemNiteligi { get; set; }
        [JsonPropertyName("girisCikisAmaci")] public string? GirisCikisAmaci { get; set; }
        [JsonPropertyName("girisCikisAmaciAciklama")] public string? GirisCikisAmaciAciklama { get; set; }
        [JsonPropertyName("esyaGeriGelmeSebebi")] public string? EsyaGeriGelmeSebebi { get; set; }
        [JsonPropertyName("esyaGeriGelmeSebebiAciklama")] public string? EsyaGeriGelmeSebebiAciklama { get; set; }
        [JsonPropertyName("yurtDisiRoyalti")] public decimal YurtDisiRoyalti { get; set; }
        [JsonPropertyName("yurtDisiRoyaltiDovizi")] public string? YurtDisiRoyaltiDovizi { get; set; }
        [JsonPropertyName("yurtDisiBanka")] public decimal YurtDisiBanka { get; set; }
        [JsonPropertyName("yurtDisiKomisyon")] public decimal YurtDisiKomisyon { get; set; }
        [JsonPropertyName("yurtDisiDepolama")] public decimal YurtDisiDepolama { get; set; }
        [JsonPropertyName("fobTutar")] public decimal FobTutar { get; set; }
        [JsonPropertyName("istatiskiKiymet")] public decimal IstatiskiKiymet { get; set; }   // cevap XML
        [JsonPropertyName("dampingKodu")] public string? DampingKodu { get; set; }        // cevap XML
        [JsonPropertyName("faturaNo")] public string? FaturaNo { get; set; }
        [JsonPropertyName("faturaTarihi")] public string? FaturaTarihi { get; set; }

        [JsonPropertyName("yurtIciHarcamalari")] public YurtIciHarcamalariDto? YurtIciHarcamalari { get; set; }
        [JsonPropertyName("odemeSekilleri")] public List<OdemeSekliDto> OdemeSekilleri { get; set; } = new();
        [JsonPropertyName("vergiler")] public List<VergiDto> Vergiler { get; set; } = new();
        [JsonPropertyName("malCinsleri")] public List<object> MalCinsleri { get; set; } = new();
        [JsonPropertyName("dokumanEdiBelgeler")] public List<DokumanDto> DokumanEdiBelgeler { get; set; } = new();
        [JsonPropertyName("konteynerBilgi")] public List<object> KonteynerBilgi { get; set; } = new();
        [JsonPropertyName("tcgbKapatmaBilgileri")] public List<object> TcgbKapatmaBilgileri { get; set; } = new();
        [JsonPropertyName("firmaEkBilgiler")] public object? FirmaEkBilgiler { get; set; } = new { };
    }

    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI DTO'LAR
    // ════════════════════════════════════════════════════════════════
    public class YurtIciHarcamalariDto
    {
        [JsonPropertyName("ardiye")] public decimal Ardiye { get; set; }
        [JsonPropertyName("tahmilTahliye")] public decimal TahmilTahliye { get; set; }
        [JsonPropertyName("bankaMasraflari")] public decimal BankaMasraflari { get; set; }
        [JsonPropertyName("kkdfMatrah")] public decimal KkdfMatrah { get; set; }
        [JsonPropertyName("kulturFonu")] public decimal KulturFonu { get; set; }
        [JsonPropertyName("diger1")] public decimal Diger1 { get; set; }
        [JsonPropertyName("diger2")] public decimal Diger2 { get; set; }
        [JsonPropertyName("yurtIciCevre")] public decimal YurtIciCevre { get; set; }
        [JsonPropertyName("digerAciklama")] public string? DigerAciklama { get; set; }
    }

    public class FirmaDto
    {
        [JsonPropertyName("no")] public int No { get; set; }
        [JsonPropertyName("tip")] public string? Tip { get; set; }
        [JsonPropertyName("accountNo")] public string? AccountNo { get; set; }
        [JsonPropertyName("vkn")] public string? Vkn { get; set; }
        [JsonPropertyName("yfksiiks")] public string? Yfksiiks { get; set; }
        [JsonPropertyName("adi")] public string? Adi { get; set; }
        [JsonPropertyName("ulkeNo")] public string? UlkeNo { get; set; }
        [JsonPropertyName("adres")] public string? Adres { get; set; }
        [JsonPropertyName("maliSorumlu")] public string? MaliSorumlu { get; set; }
        [JsonPropertyName("maliSorumluVkn")] public string? MaliSorumluVkn { get; set; }
    }

    public class OzetBeyanDto
    {
        [JsonPropertyName("ozetbeyanNo")] public string? OzetbeyanNo { get; set; }
        [JsonPropertyName("ozetbeyanTarihi")] public string? OzetbeyanTarihi { get; set; }  // cevap XML
        [JsonPropertyName("tasimaSenediNo")] public string? TasimaSenediNo { get; set; }
        [JsonPropertyName("baskamRejim")] public string? BaskamRejim { get; set; }
        [JsonPropertyName("toplamMiktar")] public decimal ToplamMiktar { get; set; }
        [JsonPropertyName("gtipNo")] public string? GtipNo { get; set; }
        [JsonPropertyName("ambarIci")] public string? AmbarIci { get; set; }
        [JsonPropertyName("ozetbeyanIslemKapsami")] public string? OzetbeyanIslemKapsami { get; set; }
    }

    public class OdemeSekliDto
    {
        [JsonPropertyName("kodu")] public int? Kodu { get; set; }
        [JsonPropertyName("tutari")] public decimal Tutari { get; set; }
        [JsonPropertyName("tranferBildirimNo")] public string? TranferBildirimNo { get; set; }
        [JsonPropertyName("tranferBildirimTarihi")] public string? TranferBildirimTarihi { get; set; }
    }

    public class VergiDto
    {
        [JsonPropertyName("siraNo")] public int SiraNo { get; set; }
        [JsonPropertyName("turu")] public string? Turu { get; set; }
        [JsonPropertyName("matrahi")] public decimal Matrahi { get; set; }
        [JsonPropertyName("orani")] public decimal Orani { get; set; }
        [JsonPropertyName("tutari")] public decimal Tutari { get; set; }
        [JsonPropertyName("odemeSekli")] public string? OdemeSekli { get; set; }
        [JsonPropertyName("vy")] public string? Vy { get; set; }
        [JsonPropertyName("adi")] public string? Adi { get; set; }
    }

    public class DokumanDto
    {
        [JsonPropertyName("kalemNo")] public int KalemNo { get; set; }
        [JsonPropertyName("kod")] public string? Kod { get; set; }
        [JsonPropertyName("tip")] public string? Tip { get; set; }
        [JsonPropertyName("tarihi")] public string? Tarihi { get; set; }
        [JsonPropertyName("referans")] public string? Referans { get; set; }
        [JsonPropertyName("tescilli")] public string? Tescilli { get; set; }
    }

    public class KiymetBildirimDto
    {
        [JsonPropertyName("aliciSaticiAyrintilar")] public string? AliciSaticiAyrintilar { get; set; }
        [JsonPropertyName("edim")] public string? Edim { get; set; }
        [JsonPropertyName("emsal")] public string? Emsal { get; set; }
        [JsonPropertyName("gumrukIdaresiKarari")] public string? GumrukIdaresiKarari { get; set; }
        [JsonPropertyName("kisitlamalar")] public string? Kisitlamalar { get; set; }
        [JsonPropertyName("kisitlamalarAyrintilar")] public string? KisitlamalarAyrintilar { get; set; }
        [JsonPropertyName("munasebet")] public string? Munasebet { get; set; }
        [JsonPropertyName("royalti")] public string? Royalti { get; set; }
        [JsonPropertyName("royaltiKosullar")] public string? RoyaltiKosullar { get; set; }
        [JsonPropertyName("saticiyaIntikal")] public string? SaticiyaIntikal { get; set; }
        [JsonPropertyName("saticiyaIntikalKosullar")] public string? SaticiyaIntikalKosullar { get; set; }
        [JsonPropertyName("sehirYer")] public string? SehirYer { get; set; }
        [JsonPropertyName("sozlesmeTarihiSayisi")] public string? SozlesmeTarihiSayisi { get; set; }
        [JsonPropertyName("faturaTarihiSayisi")] public string? FaturaTarihiSayisi { get; set; }
        [JsonPropertyName("taahutname")] public string? Taahutname { get; set; }
        [JsonPropertyName("kiymetBildirimKalemler")] public List<KiymetBildirimKalemDto> KiymetBildirimKalemler { get; set; } = new();
    }

    public class KiymetBildirimKalemDto
    {
        [JsonPropertyName("detayNo")] public int DetayNo { get; set; }
        [JsonPropertyName("digerOdemelerNiteligi")] public string? DigerOdemelerNiteligi { get; set; }
        [JsonPropertyName("dolayliIntikal")] public decimal DolayliIntikal { get; set; }
        [JsonPropertyName("dolayliOdeme")] public decimal DolayliOdeme { get; set; }
        [JsonPropertyName("girisSonrasiNakliye")] public decimal GirisSonrasiNakliye { get; set; }
        [JsonPropertyName("ithalatKatilanMalzeme")] public decimal IthalatKatilanMalzeme { get; set; }
        [JsonPropertyName("IthalatUretimAraclar")] public decimal IthalatUretimAraclar { get; set; }
        [JsonPropertyName("IthalatUretimTuketimMalzemesi")] public decimal IthalatUretimTuketimMalzemesi { get; set; }
        [JsonPropertyName("kapAmbalajBedeli")] public decimal KapAmbalajBedeli { get; set; }
        [JsonPropertyName("komisyon")] public decimal Komisyon { get; set; }
        [JsonPropertyName("teknikYardim")] public decimal TeknikYardim { get; set; }
        [JsonPropertyName("tellaliye")] public decimal Tellaliye { get; set; }
        [JsonPropertyName("vergiHarcFon")] public decimal VergiHarcFon { get; set; }
        [JsonPropertyName("planTaslak")] public decimal PlanTaslak { get; set; }
        [JsonPropertyName("royaltiLisans")] public decimal RoyaltiLisans { get; set; }
        [JsonPropertyName("nakliye")] public decimal Nakliye { get; set; }
        [JsonPropertyName("sigorta")] public decimal Sigorta { get; set; }
        [JsonPropertyName("digerOdemeler")] public decimal DigerOdemeler { get; set; }
    }

    // ════════════════════════════════════════════════════════════════
    //  CEVAP XML PARSE DTO (Evrim'in döndürdüğü sonuç)
    // ════════════════════════════════════════════════════════════════
    public class EvrimCevapDto
    {
        public string? BeyannameNo { get; set; }
        public string? TescilTarihi { get; set; }
        public string? MuayeneMemuru { get; set; }
        public decimal ToplamVergi { get; set; }
        public string? DovizKuruAlis { get; set; }
        public string? DovizKuruSatis { get; set; }
        public List<KalemCevapDto> Kalemler { get; set; } = new();

        /// <summary>
        /// cevap-41.xml (IslemSonucGetir2Result) parse eder
        /// </summary>
        public static EvrimCevapDto ParseFromCevapXml(string cevapXmlContent)
        {
            // GidenXML içindeki escaped XML'i çıkar
            var doc = XDocument.Parse(cevapXmlContent);
            XNamespace diffgr = "urn:schemas-microsoft-com:xml-diffgram-v1";
            XNamespace ms = "urn:schemas-microsoft-com:xml-msdata";

            var gidenXmlRaw = doc.Descendants("GidenXML").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(gidenXmlRaw))
                return new EvrimCevapDto();

            var sonuc = XDocument.Parse(gidenXmlRaw);
            XNamespace ns = "http://tempuri.org/";

            var dto = new EvrimCevapDto
            {
                BeyannameNo = sonuc.Root?.Element(ns + "Beyanname_no")?.Value,
                TescilTarihi = sonuc.Root?.Element(ns + "Tescil_tarihi")?.Value,
                MuayeneMemuru = sonuc.Root?.Element(ns + "Muayene_memuru")?.Value,
                DovizKuruAlis = sonuc.Root?.Element(ns + "Doviz_kuru_alis")?.Value,
                DovizKuruSatis = sonuc.Root?.Element(ns + "Doviz_kuru_satis")?.Value,
            };

            // Toplam vergi
            var toplamVergiEl = sonuc.Root?
                .Element(ns + "Toplam_vergiler")?
                .Elements(ns + "Toplam_Vergi")
                .FirstOrDefault();
            if (toplamVergiEl != null)
                decimal.TryParse(toplamVergiEl.Element(ns + "Miktar")?.Value?.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var tv);

            // Kalem bazlı istatistiki kıymet
            var istatistikiEls = sonuc.Root?
                .Element(ns + "Istatistiki_kiymetleri")?
                .Elements(ns + "Istatistiki_Kiymeti")
                .ToList();

            if (istatistikiEls != null)
            {
                foreach (var el in istatistikiEls)
                {
                    var siraNo = int.TryParse(el.Element(ns + "Kalem_no")?.Value, out var k) ? k : 1;
                    decimal.TryParse(el.Element(ns + "Miktar")?.Value?.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var kiymet);

                    dto.Kalemler.Add(new KalemCevapDto { SiraNo = siraNo, IstatistikiKiymet = kiymet });
                }
            }

            // KDV oranı vergilerden
            var kdvVergi = sonuc.Root?
                .Element(ns + "Vergiler")?
                .Elements(ns + "Vergi")
                .FirstOrDefault(v => v.Element(ns + "Kod")?.Value == "40");

            if (kdvVergi != null)
            {
                foreach (var kalemDto in dto.Kalemler)
                    kalemDto.KdvOrani = kdvVergi.Element(ns + "Oran")?.Value;
            }

            // e-Ticaret cevabı
            var eTicaretSoru = sonuc.Root?
                .Element(ns + "Soru_cevap")?
                .Elements(ns + "Soru_Cevap")
                .FirstOrDefault(sc => sc.Element(ns + "Soru_no")?.Value == "5178");
            // (üst katmana aktarılabilir)

            return dto;
        }
    }

    public class KalemCevapDto
    {
        public int SiraNo { get; set; }
        public decimal IstatistikiKiymet { get; set; }
        public string? KdvOrani { get; set; }
        public string? DampingKodu { get; set; }
    }
}