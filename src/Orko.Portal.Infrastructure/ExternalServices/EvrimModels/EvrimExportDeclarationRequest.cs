using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim yeni API - POST /api/create_export_declaration icin kullanilir.
/// ExportDeclaration semasi. ImportDeclaration'dan farkli alan adlari vardir.
/// </summary>
public class EvrimExportDeclarationRequest
{
    /// <summary>Dosya Referans Numarasi (max 50)</summary>
    [JsonPropertyName("RefNo")]
    public string? RefNo { get; set; }

    /// <summary>Karne sahibi</summary>
    [JsonPropertyName("KarneSahibi")]
    public string? KarneSahibi { get; set; }

    /// <summary>Beyan sahibi</summary>
    [JsonPropertyName("BeyanSahibi")]
    public string? BeyanSahibi { get; set; }

    /// <summary>Olusturan kullanici kodu</summary>
    [JsonPropertyName("Created_user")]
    public string? CreatedUser { get; set; }

    /// <summary>Kur ve Dosya Tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("File_date")]
    public string? FileDate { get; set; }

    /// <summary>Rejim Kodu. Orn: 1000, 3151 [ZORUNLU]</summary>
    [JsonPropertyName("Rejim")]
    public string? Rejim { get; set; }

    /// <summary>Gumruk Kodu</summary>
    [JsonPropertyName("Gumruk")]
    public int? Gumruk { get; set; }

    /// <summary>Basitlestirilmis Usul</summary>
    [JsonPropertyName("Basitlestirilmis_usul")]
    public string? BasitlestirilmisUsul { get; set; }

    /// <summary>Yuk belgeleri sayisi</summary>
    [JsonPropertyName("Yuk_belgeleri_sayisi")]
    public int? YukBelgeleriSayisi { get; set; }

    /// <summary>Kap adedi</summary>
    [JsonPropertyName("Kap_adedi")]
    public string? KapAdedi { get; set; }

    /// <summary>Detay aciklama 2</summary>
    [JsonPropertyName("DetayAcik2")]
    public string? DetayAcik2 { get; set; }

    /// <summary>Ticaret ulkesi kodu</summary>
    [JsonPropertyName("Ticaret_ulkesi")]
    public string? TicaretUlkesi { get; set; }

    /// <summary>Cikis ulkesi kodu</summary>
    [JsonPropertyName("Cikis_ulkesi")]
    public string? CikisUlkesi { get; set; }

    /// <summary>Birlik kayit numarasi</summary>
    [JsonPropertyName("Birlik_kayit_numarasi")]
    public string? BirlikKayitNumarasi { get; set; }

    /// <summary>Birlik kripto numarasi</summary>
    [JsonPropertyName("Birlik_kripto_numarasi")]
    public string? BirlikKriptoNumarasi { get; set; }

    /// <summary>Gidecegi ulke kodu</summary>
    [JsonPropertyName("Gidecegi_ulke")]
    public string? GidecegiUlke { get; set; }

    /// <summary>Gidecegi sevk ulkesi kodu</summary>
    [JsonPropertyName("Gidecegi_sevk_ulkesi")]
    public string? GidecegiSevkUlkesi { get; set; }

    /// <summary>Cikistaki aracin tipi</summary>
    [JsonPropertyName("Cikistaki_aracin_tipi")]
    public string? CikistakiAracinTipi { get; set; }

    /// <summary>Cikistaki aracin kimligi</summary>
    [JsonPropertyName("Cikistaki_aracin_kimligi")]
    public string? CikistakiAracinKimligi { get; set; }

    /// <summary>Cikistaki aracin ulkesi</summary>
    [JsonPropertyName("Cikistaki_aracin_ulkesi")]
    public string? CikistakiAracinUlkesi { get; set; }

    /// <summary>Teslim sekli - Incoterm. Orn: CIF, CFR, FOB</summary>
    [JsonPropertyName("Teslim_sekli")]
    public string? TeslimSekli { get; set; }

    /// <summary>Teslim yeri</summary>
    [JsonPropertyName("Teslim_yeri")]
    public string? TeslimYeri { get; set; }

    /// <summary>Konteyner durumu</summary>
    [JsonPropertyName("Konteyner")]
    public string? Konteyner { get; set; }

    /// <summary>Sinirdaki aracin tipi</summary>
    [JsonPropertyName("Sinirdaki_aracin_tipi")]
    public string? SinirdakiAracinTipi { get; set; }

    /// <summary>Sinirdaki aracin kimligi</summary>
    [JsonPropertyName("Sinirdaki_aracin_kimligi")]
    public string? SinirdakiAracinKimligi { get; set; }

    /// <summary>Sinirdaki aracin ulkesi</summary>
    [JsonPropertyName("Sinirdaki_aracin_ulkesi")]
    public string? SinirdakiAracinUlkesi { get; set; }

    /// <summary>Toplam fatura tutari</summary>
    [JsonPropertyName("Toplam_fatura")]
    public decimal? ToplamFatura { get; set; }

    /// <summary>Toplam navlun tutari</summary>
    [JsonPropertyName("Toplam_navlun")]
    public decimal? ToplamNavlun { get; set; }

    /// <summary>Toplam fatura dovizi. Orn: USD, EUR</summary>
    [JsonPropertyName("Toplam_fatura_dovizi")]
    public string? ToplamFaturaDovizi { get; set; }

    /// <summary>Sinirdaki tasima sekli</summary>
    [JsonPropertyName("Sinirdaki_tasima_sekli")]
    public string? SinirdakiTasimaSekli { get; set; }

    /// <summary>Alici satici iliskisi</summary>
    [JsonPropertyName("Alici_satici_iliskisi")]
    public int? AliciSaticiIliskisi { get; set; }

    /// <summary>Toplam sigorta tutari</summary>
    [JsonPropertyName("Toplam_sigorta")]
    public decimal? ToplamSigorta { get; set; }

    /// <summary>Yukleme/bosaltma yeri</summary>
    [JsonPropertyName("Yukleme_bosaltma_yeri")]
    public string? YuklemeBosaltmaYeri { get; set; }

    /// <summary>Odeme sekli kodu</summary>
    [JsonPropertyName("Odeme_sekli")]
    public int? OdemeSekli { get; set; }

    /// <summary>Banka kodu</summary>
    [JsonPropertyName("Banka_kodu")]
    public string? BankaKodu { get; set; }

    /// <summary>Esyanin bulundugu yer</summary>
    [JsonPropertyName("Esyanin_bulundugu_yer")]
    public string? EsyaninBulunduguYer { get; set; }

    /// <summary>Varis gumruk idaresi kodu</summary>
    [JsonPropertyName("Varis_gumruk_idaresi")]
    public int? VarisGumrukIdaresi { get; set; }

    /// <summary>Antrepo kodu</summary>
    [JsonPropertyName("Antrepo_kodu")]
    public string? AntrepoKodu { get; set; }

    /// <summary>Birlik referans numarasi</summary>
    [JsonPropertyName("BirlikReferans")]
    public string? BirlikReferans { get; set; }

    /// <summary>Tasarlanan guzergah</summary>
    [JsonPropertyName("Tasarlanan_guzergah")]
    public string? TasarlananGuzergah { get; set; }

    /// <summary>Tasarlanan guzergah 2</summary>
    [JsonPropertyName("Tasarlanan_guzergah2")]
    public string? TasarlananGuzergah2 { get; set; }

    /// <summary>Tasarlanan guzergah 3</summary>
    [JsonPropertyName("Tasarlanan_guzergah3")]
    public string? TasarlananGuzergah3 { get; set; }

    /// <summary>Tasarlanan guzergah 4</summary>
    [JsonPropertyName("Tasarlanan_guzergah4")]
    public string? TasarlananGuzergah4 { get; set; }

    /// <summary>Teminat sekli</summary>
    [JsonPropertyName("Teminat")]
    public string? Teminat { get; set; }

    /// <summary>Referans tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Referans_tarihi")]
    public string? ReferansTarihi { get; set; }

    /// <summary>GCB acilis tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("GCBAcilisTarihi")]
    public string? GCBAcilisTarihi { get; set; }

    /// <summary>GCB kapanis tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("GCBKapanisTarihi")]
    public string? GCBKapanisTarihi { get; set; }

    /// <summary>Vergi turu</summary>
    [JsonPropertyName("Vergi_turu")]
    public string? VergiTuru { get; set; }

    /// <summary>Vergi tutari</summary>
    [JsonPropertyName("Vergi_tutari")]
    public decimal? VergiTutari { get; set; }

    /// <summary>Telafi edici vergi</summary>
    [JsonPropertyName("Telafi_edici_vergi")]
    public decimal? TelafEdiciVergi { get; set; }

    /// <summary>Teminat odeme</summary>
    [JsonPropertyName("TOdeme")]
    public string? TOdeme { get; set; }

    /// <summary>Liman kodu</summary>
    [JsonPropertyName("LimanKodu")]
    public string? LimanKodu { get; set; }

    /// <summary>Giris gumruk idaresi</summary>
    [JsonPropertyName("Giris_gumruk_idaresi")]
    public string? GirisGumrukIdaresi { get; set; }

    /// <summary>Islemin niteligi</summary>
    [JsonPropertyName("Islemin_niteligi")]
    public string? IsleminNiteligi { get; set; }

    /// <summary>Aciklamalar</summary>
    [JsonPropertyName("Aciklamalar")]
    public string? Aciklamalar { get; set; }

    /// <summary>Kullanici kodu</summary>
    [JsonPropertyName("Kullanici_kodu")]
    public int? KullaniciKodu { get; set; }

    /// <summary>Is kodu</summary>
    [JsonPropertyName("Is_kodu")]
    public string? IsKodu { get; set; }

    /// <summary>Is plan kodu</summary>
    [JsonPropertyName("Is_Plan_Kodu")]
    public int? IsPlanKodu { get; set; }

    /// <summary>Firma bilgileri listesi</summary>
    [JsonPropertyName("Firma_Bilgi")]
    public List<EvrimCreateFirma>? FirmaBilgi { get; set; }

    /// <summary>Ozet beyanlar</summary>
    [JsonPropertyName("Ozetbeyanlar")]
    public object? Ozetbeyanlar { get; set; }

    /// <summary>Birlik kodu</summary>
    [JsonPropertyName("Birlik_kodu")]
    public string? BirlikKodu { get; set; }

    /// <summary>Birlik alt kodu</summary>
    [JsonPropertyName("Birlik_alt_kodu")]
    public string? BirlikAltKodu { get; set; }

    /// <summary>Tir/Konteyner sayisi</summary>
    [JsonPropertyName("TirKntynrSayisi")]
    public int? TirKntynrSayisi { get; set; }

    /// <summary>Ay</summary>
    [JsonPropertyName("Ay")]
    public int? Ay { get; set; }

    /// <summary>Ambalaj</summary>
    [JsonPropertyName("Ambalaj")]
    public string? Ambalaj { get; set; }

    /// <summary>Is takip kodu</summary>
    [JsonPropertyName("Is_takip_kodu")]
    public string? IsTakipKodu { get; set; }

    /// <summary>E-Ticaret</summary>
    [JsonPropertyName("eTicaret")]
    public string? ETicaret { get; set; }

    /// <summary>Siparis turu [ZORUNLU]</summary>
    [JsonPropertyName("siparisTuru")]
    public string? SiparisTuru { get; set; }

    /// <summary>Dokme yuk</summary>
    [JsonPropertyName("Dokme")]
    public bool? Dokme { get; set; }

    /// <summary>Parsiyel yuk</summary>
    [JsonPropertyName("Parsiyel")]
    public bool? Parsiyel { get; set; }

    /// <summary>TEV (Telafi Edici Vergi) tutari</summary>
    [JsonPropertyName("TEVTutar")]
    public decimal? TEVTutar { get; set; }

    /// <summary>Dosya notlari</summary>
    [JsonPropertyName("MasterNotes")]
    public object? MasterNotes { get; set; }

    /// <summary>Cut-Off tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("CutOffTarihi")]
    public string? CutOffTarihi { get; set; }

    /// <summary>Lojistik sorumlusu</summary>
    [JsonPropertyName("LojistikSorumlusu")]
    public string? LojistikSorumlusu { get; set; }

    /// <summary>Ihracat beyanname kalemleri</summary>
    [JsonPropertyName("Kalemler")]
    public List<EvrimExportKalem>? Kalemler { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  IHRACAT KALEM
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Ihracat beyanname kalem detayi. ImportDeclaration Detail'den farkli alan adlari vardir.
/// </summary>
public class EvrimExportKalem
{
    /// <summary>Detay/kalem numarasi</summary>
    [JsonPropertyName("Detay_No")]
    public int? DetayNo { get; set; }

    /// <summary>Sira numarasi</summary>
    [JsonPropertyName("SiraNo")]
    public int? SiraNo { get; set; }

    /// <summary>GTIP kodu</summary>
    [JsonPropertyName("Gtip")]
    public string? Gtip { get; set; }

    /// <summary>Mensei ulke kodu</summary>
    [JsonPropertyName("Mensei_ulke")]
    public string? MenseiUlke { get; set; }

    /// <summary>Kalem FOB tutari</summary>
    [JsonPropertyName("Kalem_FOB_Tutar")]
    public decimal? KalemFOBTutar { get; set; }

    /// <summary>Net agirlik (kg)</summary>
    [JsonPropertyName("Net_agirlik")]
    public decimal? NetAgirlik { get; set; }

    /// <summary>Brut agirlik (kg)</summary>
    [JsonPropertyName("Brut_agirlik")]
    public decimal? BrutAgirlik { get; set; }

    /// <summary>Istatistiki miktar</summary>
    [JsonPropertyName("Istatistiki_miktar")]
    public decimal? IstatistikiMiktar { get; set; }

    /// <summary>Muafiyet kodu 1</summary>
    [JsonPropertyName("Muafiyetler_1")]
    public string? Muafiyetler1 { get; set; }

    /// <summary>Muafiyet kodu 2</summary>
    [JsonPropertyName("Muafiyetler_2")]
    public string? Muafiyetler2 { get; set; }

    /// <summary>Ozellik kodu</summary>
    [JsonPropertyName("Ozellik")]
    public string? Ozellik { get; set; }

    /// <summary>Ticari tanimi</summary>
    [JsonPropertyName("Ticari_tanimi")]
    public string? TicariTanimi { get; set; }

    /// <summary>Marka</summary>
    [JsonPropertyName("Marka")]
    public string? Marka { get; set; }

    /// <summary>Marka adi</summary>
    [JsonPropertyName("Marka_adi")]
    public string? MarkaAdi { get; set; }

    /// <summary>Marka turu</summary>
    [JsonPropertyName("Marka_turu")]
    public string? MarkaTuru { get; set; }

    /// <summary>Mahrece iade</summary>
    [JsonPropertyName("Mahrece_iade")]
    public string? MahreceIade { get; set; }

    /// <summary>Kalem islem niteligi</summary>
    [JsonPropertyName("Kalem_islem_niteligi")]
    public string? KalemIslemNiteligi { get; set; }

    /// <summary>Seri/Sasi numarasi</summary>
    [JsonPropertyName("Numara")]
    public string? Numara { get; set; }

    /// <summary>Kap cinsi</summary>
    [JsonPropertyName("Cinsi")]
    public string? Cinsi { get; set; }

    /// <summary>Kap adedi</summary>
    [JsonPropertyName("Adedi")]
    public string? Adedi { get; set; }

    /// <summary>Miktar birimi</summary>
    [JsonPropertyName("Miktar_birimi")]
    public string? MiktarBirimi { get; set; }

    /// <summary>Satir numarasi</summary>
    [JsonPropertyName("Satir_no")]
    public string? SatirNo { get; set; }

    /// <summary>Miktar</summary>
    [JsonPropertyName("Miktar")]
    public decimal? Miktar { get; set; }

    /// <summary>KDV orani (%)</summary>
    [JsonPropertyName("Kdv_orani")]
    public string? KdvOrani { get; set; }

    /// <summary>44 nolu han aciklamasi</summary>
    [JsonPropertyName("Aciklama_44")]
    public string? Aciklama44 { get; set; }

    /// <summary>Not</summary>
    [JsonPropertyName("Note")]
    public string? Note { get; set; }

    /// <summary>Ithalat sekli</summary>
    [JsonPropertyName("Ithalat_sekli")]
    public string? IthalatSekli { get; set; }

    /// <summary>Giris/cikis amaci kodu</summary>
    [JsonPropertyName("Giris_Cikis_Amaci")]
    public string? GirisCikisAmaci { get; set; }

    /// <summary>Giris/cikis amaci aciklamasi</summary>
    [JsonPropertyName("Giris_Cikis_Amaci_Aciklama")]
    public string? GirisCikisAmaciAciklama { get; set; }

    /// <summary>Fatura numarasi</summary>
    [JsonPropertyName("FaturaNo")]
    public string? FaturaNo { get; set; }

    /// <summary>Fatura tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Fatura_Tarihi")]
    public string? FaturaTarihi { get; set; }

    /// <summary>Satir GUID (benzersiz kimlik)</summary>
    [JsonPropertyName("SatirGuid")]
    public string? SatirGuid { get; set; }

    /// <summary>SAP siparis numarasi</summary>
    [JsonPropertyName("SAP_ORDER_NO")]
    public string? SapOrderNo { get; set; }

    /// <summary>Satin alma siparis numarasi</summary>
    [JsonPropertyName("PURCHASE_ORDER_NUMBER")]
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>Urun aciklamasi</summary>
    [JsonPropertyName("PRODUCT_DESCRIPTION")]
    public string? ProductDescription { get; set; }

    /// <summary>ECCN (Export Control Classification Number)</summary>
    [JsonPropertyName("ECCN")]
    public string? ECCN { get; set; }

    /// <summary>Alici bilgisi</summary>
    [JsonPropertyName("CONSIGNEE")]
    public string? Consignee { get; set; }

    /// <summary>Siparis numarasi</summary>
    [JsonPropertyName("Order_No")]
    public string? OrderNo { get; set; }

    /// <summary>Siparis ID</summary>
    [JsonPropertyName("Order_ID")]
    public string? OrderID { get; set; }

    /// <summary>TCGB acma/kapatma bilgileri</summary>
    [JsonPropertyName("Tcgbacmakapatma_Bilgi")]
    public List<EvrimCreateTcgbKapatma>? TcgbKapatmaBilgi { get; set; }

    /// <summary>Dokumanlar listesi</summary>
    [JsonPropertyName("Dokumanlar")]
    public List<EvrimCreateDokuman>? Dokumanlar { get; set; }

    /// <summary>Konteyner bilgileri</summary>
    [JsonPropertyName("Konteyner_Bilgi")]
    public List<EvrimCreateKonteyner>? KonteynerBilgi { get; set; }

    /// <summary>Imalatci firma numarasi</summary>
    [JsonPropertyName("Imalatci")]
    public int? Imalatci { get; set; }

    /// <summary>Mal kodu</summary>
    [JsonPropertyName("MalKodu")]
    public string? MalKodu { get; set; }

    /// <summary>Kalem CIF birim fiyat</summary>
    [JsonPropertyName("Kalem_CIF_BF")]
    public decimal? KalemCIFBF { get; set; }

    /// <summary>Kalem FOB birim fiyat</summary>
    [JsonPropertyName("Kalem_FOB_BF")]
    public decimal? KalemFOBBF { get; set; }

    /// <summary>Kalem CIF tutar</summary>
    [JsonPropertyName("Kalem_CIF_tutar")]
    public decimal? KalemCIFTutar { get; set; }

    /// <summary>Olcu birimi</summary>
    [JsonPropertyName("UnitOfMeasure")]
    public string? UnitOfMeasure { get; set; }

    /// <summary>Detay notlari</summary>
    [JsonPropertyName("DetailNotes")]
    public object? DetailNotes { get; set; }

    /// <summary>Iskonto tutari</summary>
    [JsonPropertyName("Iskonto")]
    public decimal? Iskonto { get; set; }

    /// <summary>Esya tanimi</summary>
    [JsonPropertyName("EsyaTanimi")]
    public string? EsyaTanimi { get; set; }

    /// <summary>Istatistiki kiymet</summary>
    [JsonPropertyName("Ist_Kiymet")]
    public decimal? IstKiymet { get; set; }

    /// <summary>Alt kod</summary>
    [JsonPropertyName("AltKod")]
    public string? AltKod { get; set; }

    /// <summary>Siparis turu</summary>
    [JsonPropertyName("siparisTuru")]
    public string? SiparisTuru { get; set; }
}
