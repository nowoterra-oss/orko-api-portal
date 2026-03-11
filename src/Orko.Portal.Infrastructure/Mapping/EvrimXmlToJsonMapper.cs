using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OrkoPortal.Infrastructure.Mapping
{
    /// <summary>
    /// ERP'den üretilen <Gelen> XML'ini Evrim REST API'nin beklediği JSON modeline dönüştürür.
    /// Mapping kuralları evxim_xml.xlsx referans dosyasından alınmıştır.
    /// </summary>
    public class EvrimXmlToJsonMapper
    {
        private static readonly XNamespace Ns = "http://tempuri.org/";

        /// <summary>
        /// Ana dönüşüm metodu. XML string → EvrimBeyannameDto
        /// </summary>
        public EvrimBeyannameDto Map(string xmlContent, EvrimCevapDto? cevapDto = null)
        {
            var doc = XDocument.Parse(xmlContent);
            var root = doc.Root!;
            var beyan = root.Element(Ns + "BeyannameBilgi") ?? root.Element("BeyannameBilgi")!;

            // Namespace'i dinamik bul (bazı XML'lerde ns yoktur)
            XNamespace ns = beyan.Name.Namespace;

            var dto = new EvrimBeyannameDto();

            // ─── ANA BEYANNAME BİLGİLERİ ────────────────────────────────────
            dto.RefId           = GetVal(root, "RefID");
            dto.DosyaNo         = GetVal(beyan, ns + "Referans_no");
            dto.ReferansNo      = GetVal(beyan, ns + "Referans_no");
            dto.IsTakipKodu     = GetVal(beyan, ns + "Referans_no");

            // İhracat/İthalat ayrımı: ref no EX → ihracat, IM → ithalat
            dto.Ihracat = dto.RefId?.Contains("/EX") == true || dto.DosyaNo?.StartsWith("EX") == true;

            dto.MusteriVergi        = GetVal(beyan, ns + "Alici_vergi_no");
            dto.MusteriAccountNumber = GetVal(beyan, ns + "Alici_vergi_no");
            dto.OlusturanKullanici  = GetVal(beyan, ns + "Kullanici_kodu");
            dto.KarneSahibi         = ParseInt(GetVal(beyan, ns + "Kullanici_kodu"));

            dto.RejimKodu           = GetVal(beyan, ns + "Rejim");
            dto.Gumruk              = ParseInt(GetVal(beyan, ns + "GUMRUK"));
            dto.BasitlestirilmisUsul = GetVal(beyan, ns + "Basitlestirilmis_usul");
            dto.YukBelgeleriSayisi  = ParseInt(GetVal(beyan, ns + "Yuk_belgeleri_sayisi"));
            dto.YukBelgesi          = ParseInt(GetVal(beyan, ns + "Yuk_belgeleri_sayisi"));
            dto.KapAdedi            = GetVal(beyan, ns + "Kap_adedi");

            // Ülke kodları
            dto.IlkUlke      = GetVal(beyan, ns + "Cikis_ulkesi");
            dto.CikisUlkesi  = GetVal(beyan, ns + "Cikis_ulkesi");
            dto.GidecegiUlke = GetVal(beyan, ns + "Gidecegi_sevk_ulkesi");
            dto.VarisUlkesi  = GetVal(beyan, ns + "Gidecegi_sevk_ulkesi");
            dto.SevkUlkesi   = GetVal(beyan, ns + "Gidecegi_ulke");
            dto.TicaretUlkesi = GetVal(beyan, ns + "Ticaret_ulkesi");

            // Araç bilgileri
            dto.CikistakiAracinTipi    = GetVal(beyan, ns + "Cikistaki_aracin_tipi");
            dto.CikistakiAracinKimligi = GetVal(beyan, ns + "Cikistaki_aracin_kimligi") 
                                       ?? GetVal(beyan, ns + "Sinirdaki_aracin_kimligi");
            dto.CikistakiAracinUlkesi  = GetVal(beyan, ns + "Cikistaki_aracin_ulkesi");
            dto.SinirdakiAracinTipi    = GetVal(beyan, ns + "Sinirdaki_aracin_tipi");
            dto.SinirdakiAracinKimligi = GetVal(beyan, ns + "Sinirdaki_aracin_kimligi");
            dto.SinirdakiAracinUlkesi  = GetVal(beyan, ns + "Sinirdaki_aracin_ulkesi");
            dto.SinirdakiTasimaSekli   = GetVal(beyan, ns + "Sinirdaki_tasima_sekli");

            // Finansal
            dto.TeslimSekli           = GetVal(beyan, ns + "Teslim_sekli");
            dto.TeslimYeri            = GetVal(beyan, ns + "Teslim_yeri");
            dto.Konteyner             = GetVal(beyan, ns + "Konteyner");
            dto.ToplamFatura          = ParseDecimal(GetVal(beyan, ns + "Toplam_fatura"));
            dto.ToplamFaturaDovizi    = GetVal(beyan, ns + "Toplam_fatura_dovizi");
            dto.ToplamNavlun          = ParseDecimal(GetVal(beyan, ns + "Toplam_navlun"));
            dto.ToplamNavlunDovizi    = GetVal(beyan, ns + "Toplan_navlun_dovizi");
            dto.AliciSaticiIliskisi   = ParseInt(GetVal(beyan, ns + "Alici_satici_iliskisi"));
            dto.ToplamSigorta         = ParseDecimal(GetVal(beyan, ns + "Toplam_sigorta"));
            dto.ToplamSigortaDovizi   = GetVal(beyan, ns + "Toplam_sigorta_dovizi");
            dto.ToplamYurtDisiHarcamalari       = ParseDecimal(GetVal(beyan, ns + "Toplam_yurt_disi_harcamalar"));
            dto.ToplamYurtDisiHarcamalariDovizi = GetVal(beyan, ns + "Toplam_yurt_disi_harcamalarin_dovizi");
            dto.ToplamYurtDisiHarcamalariAciklama = GetVal(beyan, ns + "Toplam_yurt_disi_harcamalar");
            dto.YuklemeBosaltmaYeri   = GetVal(beyan, ns + "Esyanin_bulundugu_yer");
            dto.YurtDisiRoyalti       = ParseDecimal(GetVal(beyan, ns + "YurtDisi_Royalti") ?? "0");
            dto.YurtDisiRoyaltiDovizi = GetVal(beyan, ns + "YurtDisi_Royalti_Dovizi");
            dto.YurtDisiKomisyon      = ParseDecimal(GetVal(beyan, ns + "YurtDisi_Komisyon") ?? "0");
            dto.YurtDisiDepolama      = ParseDecimal(GetVal(beyan, ns + "YurtDisi_Demuraj") ?? "0");
            dto.OdemeSekli            = GetVal(beyan, ns + "Odeme_sekli");
            dto.BankaKodu             = GetVal(beyan, ns + "Banka_kodu");
            dto.VarisGumrukIdaresi    = ParseInt(GetVal(beyan, ns + "Varis_gumruk_idaresi"));
            dto.AntrepoKodu           = GetVal(beyan, ns + "Antrepo_kodu");
            dto.EsyaninBulunduguYer   = GetVal(beyan, ns + "Esyanin_bulundugu_yer");
            dto.LimanKodu             = GetVal(beyan, ns + "LimanKodu");
            dto.GirisGumrukIdaresi    = GetVal(beyan, ns + "Giris_gumruk_idaresi");
            dto.IsleminNiteligi       = GetVal(beyan, ns + "Islemin_niteligi");
            dto.Aciklamalar           = GetVal(beyan, ns + "Aciklamalar");
            dto.TevTutar              = ParseDecimal(GetVal(beyan, ns + "Telafi_edici_vergi") ?? "0");
            dto.KkdfMatrah            = ParseDecimal(GetVal(beyan, ns + "Yurtici_Kkdf") ?? "0");
            dto.ToplamBrutAgirlik     = ParseDecimal(GetVal(beyan, ns + "Brut_agirlik") ?? "0");
            dto.ToplamNetAgirlik      = ParseDecimal(GetVal(beyan, ns + "Net_agirlik") ?? "0");
            dto.BrutAgirlik           = dto.ToplamBrutAgirlik;
            dto.NetAgirlik            = dto.ToplamNetAgirlik;
            dto.BirlikKayitNumarasi   = GetVal(beyan, ns + "Birlik_kayit_numarasi");
            dto.BirlikKriptoNumarasi  = GetVal(beyan, ns + "Birlik_kripto_numarasi");
            dto.AcentaBildirimNo      = GetVal(beyan, ns + "AcentaSevkBildirimNo");
            dto.GumrukSaymanlikVergiNo = GetVal(beyan, ns + "OdeSaymanlikBilgi");
            dto.FazlaMesaiTescilNo    = GetVal(beyan, ns + "FazlaMesaiID");
            dto.BeyanSahibiVergiNo    = GetVal(beyan, ns + "Beyan_sahibi_vergi_no");
            dto.MusavirVergiNo        = GetVal(beyan, ns + "Musavir_vergi_no");
            dto.SiparisTuru           = GetVal(beyan, ns + "SiparisTuru")
                                     ?? GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "SiparisTuru");

            // Sabit değerler (Orko'ya özel)
            dto.BeyanSahibiUnvan = "ORKO GÜMRÜKLEME";
            dto.KarnSahibiKodu   = "02";
            dto.KarneSahibiAdi   = "M/34/09540 SELÇUK ŞAHİN";

            // Cevap XML'den gelecek alanlar (ilk gönderimdeyken null olacak)
            if (cevapDto != null)
            {
                dto.BeyannameNo    = cevapDto.BeyannameNo;
                dto.BeyannameTarihi = cevapDto.TescilTarihi;
                dto.DosyaTarihi    = cevapDto.TescilTarihi;
                dto.MuayeneMemuru  = cevapDto.MuayeneMemuru;
                dto.ToplamVergi    = cevapDto.ToplamVergi;
            }

            // ─── YURTİÇİ HARCAMALAR ────────────────────────────────────────
            dto.YurtIciHarcamalari = new YurtIciHarcamalariDto
            {
                Ardiye         = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Depolama") ?? "0"),
                TahmilTahliye  = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Liman") ?? "0"),
                BankaMasraflari = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Banka") ?? "0"),
                KkdfMatrah     = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Kkdf") ?? "0"),
                KulturFonu     = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Kultur") ?? "0"),
                Diger1         = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Diger") ?? "0"),
                YurtIciCevre   = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Cevre") ?? "0"),
                DigerAciklama  = GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Yurtici_Diger_Aciklama"),
            };

            dto.ToplamYurtIciHarcamalari = ParseDecimal(GetVal(beyan, ns + "Toplam_yurt_ici_harcamalar") ?? "0");

            // ─── TEMİNAT ────────────────────────────────────────────────────
            var teminatEl = beyan.Element(ns + "Teminat")?.Element(ns + "Teminat");
            if (teminatEl != null)
            {
                dto.TeminatSekli              = GetVal(teminatEl, ns + "Teminat_sekli");
                dto.TeminatDigerTutarReferansi = GetVal(teminatEl, ns + "Diger_tutar_referansi");
                dto.TeminatGlobalGarantiNo    = GetVal(teminatEl, ns + "Global_teminat_no");
                dto.TeminatOrani              = ParseDecimal(GetVal(teminatEl, ns + "Teminat_orani") ?? "0");
                dto.TeminatAciklama           = GetVal(teminatEl, ns + "Aciklama");
                dto.TeminatOdeme              = ParseDecimal(GetVal(teminatEl, ns + "Nakdi_teminat_tutari") ?? "0");

                // Teminat değeri açıklamadan parse et: "GTR1 2524.46" → 2524.46
                if (dto.TeminatAciklama != null)
                {
                    var parts = dto.TeminatAciklama.Split(' ');
                    if (parts.Length > 1 && decimal.TryParse(parts[^1],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var tv))
                        dto.TeminatDeger = tv;
                }
            }

            // ─── ÖZET BEYANLAR ──────────────────────────────────────────────
            dto.OzetBeyanlar = beyan
                .Element(ns + "Ozetbeyanlar")?
                .Elements(ns + "Ozetbeyan")
                .Select(ob => new OzetBeyanDto
                {
                    OzetbeyanNo           = GetVal(ob, ns + "Ozetbeyan_no"),
                    TasimaSenediNo        = GetElVal(ob, ns + "ozbyacma_bilgi", ns + "tasimasenetleri", ns + "Tasima_senedi_no"),
                    BaskamRejim           = GetVal(ob, ns + "Baska_rejim"),
                    GtipNo                = GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Gtip"),
                    ToplamMiktar          = ParseDecimal(GetElVal(beyan, ns + "Kalemler", ns + "kalem", ns + "Miktar") ?? "0"),
                    // Tarih cevap XML'den gelecek
                }).ToList() ?? new List<OzetBeyanDto>();

            // ─── KALEMLERLer ────────────────────────────────────────────────
            dto.Kalemler = beyan
                .Element(ns + "Kalemler")?
                .Elements(ns + "kalem")
                .Select((kalem, idx) => MapKalem(kalem, ns, idx + 1, cevapDto))
                .ToList() ?? new List<KalemDto>();

            // ─── KIYMETBİLDİRİM ─────────────────────────────────────────────
            var kiymet = beyan.Element(ns + "KiymetBildirim")?.Element(ns + "Kiymet");
            if (kiymet != null)
            {
                dto.KiymetBildirim = new KiymetBildirimDto
                {
                    AliciSaticiAyrintilar    = GetVal(kiymet, ns + "AliciSaticiAyrintilar"),
                    Edim                     = GetVal(kiymet, ns + "Edim"),
                    GumrukIdaresiKarari      = GetVal(kiymet, ns + "GumrukIdaresiKarari"),
                    Kisitlamalar             = GetVal(kiymet, ns + "Kisitlamalar"),
                    KisitlamalarAyrintilar   = GetVal(kiymet, ns + "KisitlamalarAyrintilar"),
                    Royalti                  = GetVal(kiymet, ns + "Royalti"),
                    RoyaltiKosullar          = GetVal(kiymet, ns + "RoyaltiKosullar"),
                    SaticiyaIntikal          = GetVal(kiymet, ns + "SaticiyaIntikal"),
                    SaticiyaIntikalKosullar  = GetVal(kiymet, ns + "SaticiyaIntikalKosullar"),
                    SehirYer                 = GetVal(kiymet, ns + "SehirYer"),
                    FaturaTarihiSayisi       = GetVal(kiymet, ns + "FaturaTarihiSayisi"),
                    SozlesmeTarihiSayisi     = GetVal(kiymet, ns + "SozlesmeTarihiSayisi"),
                    Taahutname               = GetVal(kiymet, ns + "Taahutname"),
                    Munasebet                = GetVal(kiymet, ns + "AliciSatici"),

                    KiymetBildirimKalemler = kiymet
                        .Element(ns + "KiymetKalemler")?
                        .Elements(ns + "KiymetKalem")
                        .Select(kk => new KiymetBildirimKalemDto
                        {
                            DolayliOdeme              = ParseDecimal(GetVal(kk, ns + "DolayliOdeme") ?? "0"),
                            Komisyon                  = ParseDecimal(GetVal(kk, ns + "Komisyon") ?? "0"),
                            Tellaliye                 = ParseDecimal(GetVal(kk, ns + "Tellaliye") ?? "0"),
                            KapAmbalajBedeli          = ParseDecimal(GetVal(kk, ns + "KapAmbalajBedeli") ?? "0"),
                            IthalatKatilanMalzeme     = ParseDecimal(GetVal(kk, ns + "IthalaKatilanMalzeme") ?? "0"),
                            IthalatUretimAraclar      = ParseDecimal(GetVal(kk, ns + "IthalaUretimAraclar") ?? "0"),
                            IthalatUretimTuketimMalzemesi = ParseDecimal(GetVal(kk, ns + "IthalaUretimTuketimMalzemesi") ?? "0"),
                            PlanTaslak                = ParseDecimal(GetVal(kk, ns + "PlanTaslak") ?? "0"),
                            RoyaltiLisans             = ParseDecimal(GetVal(kk, ns + "RoyaltiLisans") ?? "0"),
                            DolayliIntikal            = ParseDecimal(GetVal(kk, ns + "DolayliIntikal") ?? "0"),
                            Nakliye                   = ParseDecimal(GetVal(kk, ns + "Nakliye") ?? "0"),
                            Sigorta                   = ParseDecimal(GetVal(kk, ns + "Sigorta") ?? "0"),
                            GirisSonrasiNakliye       = ParseDecimal(GetVal(kk, ns + "GirisSonrasiNakliye") ?? "0"),
                            TeknikYardim              = ParseDecimal(GetVal(kk, ns + "TeknikYardim") ?? "0"),
                            DigerOdemeler             = ParseDecimal(GetVal(kk, ns + "DigerOdemeler") ?? "0"),
                            DigerOdemelerNiteligi     = GetVal(kk, ns + "DigerOdemelerNiteligi"),
                            VergiHarcFon              = ParseDecimal(GetVal(kk, ns + "VergiHarcFon") ?? "0"),
                        })
                        .ToList() ?? new List<KiymetBildirimKalemDto>()
                };
            }

            // ─── FİRMALAR ───────────────────────────────────────────────────
            dto.Firmalar = beyan
                .Element(ns + "Firma_bilgi")?
                .Elements(ns + "firma")
                .Select(f => new FirmaDto
                {
                    Tip     = GetVal(f, ns + "Tip"),
                    Yfksiiks = GetVal(f, ns + "No"),
                    Adres   = GetVal(f, ns + "Cadde_s_no"),
                    Vkn     = GetVal(f, ns + "Kimlik_turu"),
                })
                .ToList() ?? new List<FirmaDto>();

            return dto;
        }

        // ─── KALEM MAPPER ───────────────────────────────────────────────────
        private KalemDto MapKalem(XElement kalem, XNamespace ns, int siraNo, EvrimCevapDto? cevap)
        {
            var dto = new KalemDto
            {
                GtipNo                  = GetVal(kalem, ns + "Gtip"),
                MenseiUlke              = GetVal(kalem, ns + "Mensei_ulke"),
                BrutAgirlik             = ParseDecimal(GetVal(kalem, ns + "Brut_agirlik") ?? "0"),
                NetAgirlik              = ParseDecimal(GetVal(kalem, ns + "Net_agirlik") ?? "0"),
                TamamlayiciOlcuBirimi   = GetVal(kalem, ns + "Tamamlayici_olcu_birimi"),
                IstatistikiMiktar       = ParseDecimal(GetVal(kalem, ns + "Istatistiki_miktar") ?? "0"),
                UluslararasiAnlasma     = GetVal(kalem, ns + "Uluslararasi_anlasma"),
                AlgilamaMiktari1        = ParseDecimal(GetVal(kalem, ns + "Algilama_miktari_1") ?? "0"),
                AlgilamaBirimi1         = GetVal(kalem, ns + "Algilama_birimi_1"),
                AlgilamaMiktari2        = ParseDecimal(GetVal(kalem, ns + "Algilama_miktari_2") ?? "0"),
                AlgilamaBirimi2         = GetVal(kalem, ns + "Algilama_birimi_2"),
                AlgilamaMiktari3        = ParseDecimal(GetVal(kalem, ns + "Algilama_miktari_3") ?? "0"),
                AlgilamaBirimi3         = GetVal(kalem, ns + "Algilama_birimi_3"),
                MuafKod                 = GetVal(kalem, ns + "Muafiyetler_1"),
                MuafKod2                = GetVal(kalem, ns + "Muafiyetler_2"),
                MuafKod3                = GetVal(kalem, ns + "Muafiyetler_3"),
                MuafKod4                = GetVal(kalem, ns + "Muafiyetler_4"),
                MuafKod5                = GetVal(kalem, ns + "Muafiyetler_5"),
                EkKod                   = GetVal(kalem, ns + "Ek_kod"),
                Ozellik                 = GetVal(kalem, ns + "Ozellik"),
                KalemFiyati             = ParseDecimal(GetVal(kalem, ns + "Fatura_miktari") ?? "0"),
                NavlunMiktari           = ParseDecimal(GetVal(kalem, ns + "Navlun_miktari") ?? "0"),
                SigortaMiktari          = ParseDecimal(GetVal(kalem, ns + "Sigorta_miktari") ?? "0"),
                TicariTanim             = GetVal(kalem, ns + "Ticari_tanimi"),
                EsyaTanimi1             = GetVal(kalem, ns + "Ticari_tanimi"),
                Cinsi                   = GetVal(kalem, ns + "Cinsi"),
                Adedi                   = GetVal(kalem, ns + "Adedi"),
                MiktarBirimi            = GetVal(kalem, ns + "Miktar_birimi"),
                IkincilIslem            = GetVal(kalem, ns + "Ikincil_islem"),
                TesvikNo                = GetVal(kalem, ns + "Satir_no"),
                Miktar                  = ParseDecimal(GetVal(kalem, ns + "Miktar") ?? "0"),
                KullanilmisEsya         = GetVal(kalem, ns + "Kullanilmis_esya"),
                Marka                   = GetVal(kalem, ns + "Marka"),
                Numara                  = GetVal(kalem, ns + "Numara"),
                Doviz                   = GetVal(kalem, ns + "Fatura_miktarinin_dovizi"),
                KalemIslemNiteligi      = GetVal(kalem, ns + "Kalem_Islem_Niteligi"),
                GirisCikisAmaci         = GetVal(kalem, ns + "Giris_Cikis_Amaci"),
                GirisCikisAmaciAciklama = GetVal(kalem, ns + "Giris_Cikis_Amaci_Aciklama"),
                EsyaGeriGelmeSebebi     = GetVal(kalem, ns + "EsyaGeriGelmeSebebi"),
                EsyaGeriGelmeSebebiAciklama = GetVal(kalem, ns + "EsyaGeriGelmeSebebiAciklamasi"),
                YurtDisiRoyalti         = ParseDecimal(GetVal(kalem, ns + "YurtDisi_Royalti") ?? "0"),
                YurtDisiRoyaltiDovizi   = GetVal(kalem, ns + "YurtDisi_Royalti_Dovizi"),
                YurtDisiKomisyon        = ParseDecimal(GetVal(kalem, ns + "YurtDisi_Komisyon") ?? "0"),
                YurtDisiDepolama        = ParseDecimal(GetVal(kalem, ns + "YurtDisi_Demuraj") ?? "0"),
                FobTutar                = ParseDecimal(GetVal(kalem, ns + "Fatura_miktari") ?? "0"),
                YdHesaplama             = GetVal(kalem, ns + "Toplam_yurt_disi_harcamalar"),

                // Fatura no/tarih: FaturaTarihiSayisi = "27/02/2026 GTSEXT1221" formatında
                FaturaNo    = ParseFaturaNo(GetVal(kalem.Parent?.Parent, ns + "KiymetBildirim")
                              ?? GetElVal(kalem.Parent?.Parent, ns + "KiymetBildirim", ns + "Kiymet", ns + "FaturaTarihiSayisi")),
                FaturaTarihi = ParseFaturaTarihi(GetElVal(kalem.Parent?.Parent,
                              ns + "KiymetBildirim", ns + "Kiymet", ns + "FaturaTarihiSayisi")),

                // Ödeme şekilleri
                OdemeSekilleri = kalem
                    .Element(ns + "OdemeSekilleri")?
                    .Elements(ns + "OdemeSekli")
                    .Select(os => new OdemeSekliDto
                    {
                        Kodu   = ParseInt(GetVal(os, ns + "OdemeSekliKodu")),
                        Tutari = ParseDecimal(GetVal(os, ns + "OdemeTutari") ?? "0"),
                        TranferBildirimNo = GetVal(os, ns + "TBFID"),
                    })
                    .ToList() ?? new List<OdemeSekliDto>(),

                // YurtIçi harcamalar (kalem seviyesi)
                YurtIciHarcamalari = new YurtIciHarcamalariDto
                {
                    Ardiye          = ParseDecimal(GetVal(kalem, ns + "Yurtici_Depolama") ?? "0"),
                    TahmilTahliye   = ParseDecimal(GetVal(kalem, ns + "Yurtici_Tahliye") ?? "0"),
                    BankaMasraflari = ParseDecimal(GetVal(kalem, ns + "Yurtici_Banka") ?? "0"),
                    KkdfMatrah      = ParseDecimal(GetVal(kalem, ns + "Yurtici_Kkdf") ?? "0"),
                    KulturFonu      = ParseDecimal(GetVal(kalem, ns + "Yurtici_Kultur") ?? "0"),
                    Diger1          = ParseDecimal(GetVal(kalem, ns + "Yurtici_Diger") ?? "0"),
                    YurtIciCevre    = ParseDecimal(GetVal(kalem, ns + "Yurtici_Cevre") ?? "0"),
                    DigerAciklama   = GetVal(kalem, ns + "Yurtici_Diger_Aciklama"),
                },
            };

            // Cevap XML'den gelen alanlar
            if (cevap != null)
            {
                var kalemCevap = cevap.Kalemler?.FirstOrDefault(k => k.SiraNo == siraNo);
                if (kalemCevap != null)
                {
                    dto.KdvOrani        = kalemCevap.KdvOrani;
                    dto.IstatiskiKiymet = kalemCevap.IstatistikiKiymet;
                    dto.DampingKodu     = kalemCevap.DampingKodu;
                }
            }

            return dto;
        }

        // ─── YARDIMCI METODLAR ──────────────────────────────────────────────

        private static string? GetVal(XElement? parent, XName name)
        {
            var val = parent?.Element(name)?.Value?.Trim();
            return string.IsNullOrEmpty(val) ? null : val;
        }

        private static string? GetVal(XElement? parent, string name)
        {
            if (parent == null) return null;
            // Namespace'siz dene
            var el = parent.Element(name) ?? parent.Elements().FirstOrDefault(e => e.Name.LocalName == name);
            var val = el?.Value?.Trim();
            return string.IsNullOrEmpty(val) ? null : val;
        }

        private static string? GetElVal(XElement? parent, XName container, XName child, XName field)
        {
            return GetVal(parent?.Element(container)?.Element(child), field);
        }

        private static string? GetElVal(XElement? parent, XName container, XName field)
        {
            return GetVal(parent?.Element(container), field);
        }

        private static decimal ParseDecimal(string? val)
        {
            if (string.IsNullOrEmpty(val)) return 0;
            return decimal.TryParse(val.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0;
        }

        private static int? ParseInt(string? val)
        {
            if (string.IsNullOrEmpty(val)) return null;
            return int.TryParse(val, out var i) ? i : null;
        }

        /// <summary>"27/02/2026 GTSEXT1221" → "GTSEXT1221"</summary>
        private static string? ParseFaturaNo(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            var parts = raw.Trim().Split(' ');
            return parts.Length > 1 ? parts[^1] : null;
        }

        /// <summary>"27/02/2026 GTSEXT1221" → "27/02/2026"</summary>
        private static string? ParseFaturaTarihi(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            return raw.Trim().Split(' ')[0];
        }
    }
}