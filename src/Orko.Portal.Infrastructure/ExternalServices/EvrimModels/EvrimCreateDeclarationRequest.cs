using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim yeni API - POST /api/create_export_declaration ve /api/create_import_declaration icin kullanilir.
/// Alan adlari Evrim'in yeni swagger semasina birebir uyar.
/// </summary>
public class EvrimCreateDeclarationRequest
{
    /// <summary>T: Ithalat, H: Ihracat, T-ANT: Antrepo</summary>
    [JsonPropertyName("dosyaTipi")]
    public string? DosyaTipi { get; set; }

    /// <summary>Dosya Referans Numarasi (max 50)</summary>
    [JsonPropertyName("RefNo")]
    public string? RefNo { get; set; }

    /// <summary>Olusturan kullanici kodu</summary>
    [JsonPropertyName("Created_user")]
    public string? CreatedUser { get; set; }

    /// <summary>Kur ve Dosya Tarihi (yyyy-MM-ddTHH:mm:ss)</summary>
    [JsonPropertyName("File_date")]
    public string? FileDate { get; set; }

    /// <summary>true: Ihracat, false/null: Ithalat</summary>
    [JsonPropertyName("Ihracat")]
    public bool? Ihracat { get; set; }

    /// <summary>Rejim Kodu. Orn: 4000, 3151</summary>
    [JsonPropertyName("RegimeCode")]
    public string? RegimeCode { get; set; }

    /// <summary>Gumruk Kodu. Orn: 590300</summary>
    [JsonPropertyName("Gumruk")]
    public int? Gumruk { get; set; }

    /// <summary>Basitlestirilmis Usul (max 2)</summary>
    [JsonPropertyName("Basitlestirilmis_usul")]
    public string? BasitlestirilmisUsul { get; set; }

    /// <summary>Yuk belgeleri sayisi</summary>
    [JsonPropertyName("Yuk_belgeleri_sayisi")]
    public int? YukBelgeleriSayisi { get; set; }

    /// <summary>Gidecegi Ulke Kodu (max 4)</summary>
    [JsonPropertyName("ToCountryCode")]
    public string? ToCountryCode { get; set; }

    /// <summary>Cikis Ulkesi Kodu (max 4)</summary>
    [JsonPropertyName("ExitCountryCode")]
    public string? ExitCountryCode { get; set; }

    /// <summary>Ihracat yapilan ulke kodu (max 4)</summary>
    [JsonPropertyName("ExportFromCountryCode")]
    public string? ExportFromCountryCode { get; set; }

    /// <summary>Cikistaki aracin tipi (max 2)</summary>
    [JsonPropertyName("InternalVehicleTypeCode")]
    public string? InternalVehicleTypeCode { get; set; }

    /// <summary>Cikistaki aracin ulkesi (max 4)</summary>
    [JsonPropertyName("Cikistaki_aracin_ulkesi")]
    public string? CikistakiAracinUlkesi { get; set; }

    /// <summary>Teslim sekli - Incoterm. Orn: CIF, CFR, FOB (max 3)</summary>
    [JsonPropertyName("IncotermCode")]
    public string? IncotermCode { get; set; }

    /// <summary>Konteyner durumu (max 1)</summary>
    [JsonPropertyName("Konteyner")]
    public string? Konteyner { get; set; }

    /// <summary>Sinirdaki aracin tipi (max 4)</summary>
    [JsonPropertyName("Sinirdaki_aracin_tipi")]
    public string? SinirdakiAracinTipi { get; set; }

    /// <summary>Sinirdaki aracin kimligi (max 50)</summary>
    [JsonPropertyName("BorderVehicle")]
    public string? BorderVehicle { get; set; }

    /// <summary>Sinirdaki aracin ulkesi (max 4)</summary>
    [JsonPropertyName("Sinirdaki_aracin_ulkesi")]
    public string? SinirdakiAracinUlkesi { get; set; }

    /// <summary>Toplam fatura tutari</summary>
    [JsonPropertyName("Toplam_fatura")]
    public decimal? ToplamFatura { get; set; }

    /// <summary>Doviz cinsi. Orn: USD, EUR (max 3)</summary>
    [JsonPropertyName("CurrencyTypeCode")]
    public string? CurrencyTypeCode { get; set; }

    /// <summary>Tahmini varis tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("TahminiVarisTarihi")]
    public string? TahminiVarisTarihi { get; set; }

    /// <summary>Toplam navlun tutari</summary>
    [JsonPropertyName("Freight")]
    public decimal? Freight { get; set; }

    /// <summary>Navlun doviz cinsi (max 3)</summary>
    [JsonPropertyName("FreightCurrencyType")]
    public string? FreightCurrencyType { get; set; }

    /// <summary>Sinirdaki tasima sekli (max 2)</summary>
    [JsonPropertyName("BorderVehicleTypeCode")]
    public string? BorderVehicleTypeCode { get; set; }

    /// <summary>Alici satici iliskisi</summary>
    [JsonPropertyName("Alici_satici_iliskisi")]
    public int? AliciSaticiIliskisi { get; set; }

    /// <summary>Toplam sigorta tutari</summary>
    [JsonPropertyName("Insurance")]
    public decimal? Insurance { get; set; }

    /// <summary>Sigorta doviz cinsi (max 3)</summary>
    [JsonPropertyName("InsuranceCurrencyType")]
    public string? InsuranceCurrencyType { get; set; }

    /// <summary>Toplam yurt disi harcamalar tutari</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalar")]
    public decimal? ToplamYurtDisiHarcamalar { get; set; }

    /// <summary>Yurt disi harcamalar dovizi (max 3)</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_dovizi")]
    public string? ToplamYurtDisiHarcamalarinDovizi { get; set; }

    /// <summary>Yurt disi harcamalar aciklama (max 10)</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_aciklama")]
    public string? ToplamYurtDisiHarcamalarinAciklama { get; set; }

    /// <summary>Teslim yeri (max 30)</summary>
    [JsonPropertyName("Teslim_yeri")]
    public string? TeslimYeri { get; set; }

    /// <summary>Cikistaki aracin kimligi (max 50)</summary>
    [JsonPropertyName("Cikistaki_aracin_kimligi")]
    public string? CikistakiAracinKimligi { get; set; }

    /// <summary>Kap adedi (max 10)</summary>
    [JsonPropertyName("Kap_adedi")]
    public string? KapAdedi { get; set; }

    /// <summary>Yukleme/bosaltma yeri (max 50)</summary>
    [JsonPropertyName("Yukleme_bosaltma_yeri")]
    public string? YuklemeBosaltmaYeri { get; set; }

    /// <summary>Detay aciklama 2 (max 250)</summary>
    [JsonPropertyName("DetayAcik2")]
    public string? DetayAcik2 { get; set; }

    /// <summary>Yurt disi royalti tutari</summary>
    [JsonPropertyName("YurtDisi_Royalti")]
    public decimal? YurtDisiRoyalti { get; set; }

    /// <summary>Yurt disi royalti dovizi (max 3)</summary>
    [JsonPropertyName("YurtDisi_Royalti_Dovizi")]
    public string? YurtDisiRoyaltiDovizi { get; set; }

    /// <summary>Ithal harci tutari</summary>
    [JsonPropertyName("Ithal_Harci")]
    public decimal? IthalHarci { get; set; }

    /// <summary>Yurt disi banka masraflari</summary>
    [JsonPropertyName("YurtDisi_Banka")]
    public decimal? YurtDisiBanka { get; set; }

    /// <summary>Yurt disi komisyon</summary>
    [JsonPropertyName("YurtDisi_Komisyon")]
    public decimal? YurtDisiKomisyon { get; set; }

    /// <summary>Yurt disi depolama</summary>
    [JsonPropertyName("YurtDisi_Depolama")]
    public decimal? YurtDisiDepolama { get; set; }

    /// <summary>Yurt ici harcamalar detayi</summary>
    [JsonPropertyName("Yurt_ici_harcamalar")]
    public EvrimCreateYurtIciHarcamalar? YurtIciHarcamalar { get; set; }

    /// <summary>Odeme sekli kodu</summary>
    [JsonPropertyName("PaymentTypeCode")]
    public int? PaymentTypeCode { get; set; }

    /// <summary>Banka kodu (max 12)</summary>
    [JsonPropertyName("BankCode")]
    public string? BankCode { get; set; }

    /// <summary>Antrepo firma kodu (max 10)</summary>
    [JsonPropertyName("BondedWarehouseFirmCode")]
    public string? BondedWarehouseFirmCode { get; set; }

    /// <summary>Giris gumruk idaresi kodu</summary>
    [JsonPropertyName("ImportCustomsCode")]
    public int? ImportCustomsCode { get; set; }

    /// <summary>Antrepo hedef</summary>
    [JsonPropertyName("BWTo")]
    public string? BWTo { get; set; }

    /// <summary>Esyanin bulundugu yer (max 30)</summary>
    [JsonPropertyName("Esyanin_bulundugu_yer")]
    public string? EsyaninBulunduguYer { get; set; }

    /// <summary>Liman kodu (max 15)</summary>
    [JsonPropertyName("LimanKodu")]
    public string? LimanKodu { get; set; }

    /// <summary>Varis gumruk idaresi kodu</summary>
    [JsonPropertyName("DestinationCustomsCode")]
    public int? DestinationCustomsCode { get; set; }

    /// <summary>Islemin niteligi (max 3)</summary>
    [JsonPropertyName("Islemin_niteligi")]
    public string? IsleminNiteligi { get; set; }

    /// <summary>Aciklamalar (max 250)</summary>
    [JsonPropertyName("Aciklamalar")]
    public string? Aciklamalar { get; set; }

    /// <summary>Kullanici kodu</summary>
    [JsonPropertyName("Kullanici_kodu")]
    public int? KullaniciKodu { get; set; }

    /// <summary>Birlik referans numarasi (max 50)</summary>
    [JsonPropertyName("BirlikReferans")]
    public string? BirlikReferans { get; set; }

    /// <summary>Tir/Konteyner sayisi</summary>
    [JsonPropertyName("TirKntynrSayisi")]
    public int? TirKntynrSayisi { get; set; }

    /// <summary>Sigorta yuzde orani</summary>
    [JsonPropertyName("SigortaYuzde")]
    public decimal? SigortaYuzde { get; set; }

    /// <summary>Navlun yuzde orani</summary>
    [JsonPropertyName("NavlunYuzde")]
    public decimal? NavlunYuzde { get; set; }

    /// <summary>Transit GA (max 1)</summary>
    [JsonPropertyName("TrnsitGA")]
    public string? TrnsitGA { get; set; }

    /// <summary>Tasarlanan guzergah (max 20)</summary>
    [JsonPropertyName("Tasarlanan_guzergah")]
    public string? TasarlananGuzergah { get; set; }

    /// <summary>Tasarlanan guzergah 2 (max 20)</summary>
    [JsonPropertyName("Tasarlanan_guzergah2")]
    public string? TasarlananGuzergah2 { get; set; }

    /// <summary>Tasarlanan guzergah 3 (max 20)</summary>
    [JsonPropertyName("Tasarlanan_guzergah3")]
    public string? TasarlananGuzergah3 { get; set; }

    /// <summary>Tasarlanan guzergah 4 (max 20)</summary>
    [JsonPropertyName("Tasarlanan_guzergah4")]
    public string? TasarlananGuzergah4 { get; set; }

    /// <summary>Teminat sekli (max 80)</summary>
    [JsonPropertyName("Teminat")]
    public string? Teminat { get; set; }

    /// <summary>Teminat tipi (max 35)</summary>
    [JsonPropertyName("TTip")]
    public string? TTip { get; set; }

    /// <summary>Teminat diger tutar referansi (max 35)</summary>
    [JsonPropertyName("TDigerRefNo")]
    public string? TDigerRefNo { get; set; }

    /// <summary>Global garanti numarasi (max 35)</summary>
    [JsonPropertyName("TGlobalGarantiNo")]
    public string? TGlobalGarantiNo { get; set; }

    /// <summary>Teminat orani yuzde</summary>
    [JsonPropertyName("TYuzde")]
    public decimal? TYuzde { get; set; }

    /// <summary>Teminat aciklama (max 35)</summary>
    [JsonPropertyName("TAciklama")]
    public string? TAciklama { get; set; }

    /// <summary>Indirimli teminat</summary>
    [JsonPropertyName("IndirimliTeminat")]
    public bool? IndirimliTeminat { get; set; }

    /// <summary>Teminat odeme (max 1)</summary>
    [JsonPropertyName("TOdeme")]
    public string? TOdeme { get; set; }

    /// <summary>Sure sonu tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("SureSonuTarih")]
    public string? SureSonuTarih { get; set; }

    /// <summary>Tahmini baslangic tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("TahminiBaslangicTar")]
    public string? TahminiBaslangicTar { get; set; }

    /// <summary>Tahmini bitis tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("TahminiBitisTarihi")]
    public string? TahminiBitisTarihi { get; set; }

    /// <summary>Siparis numarasi 1 (max 20)</summary>
    [JsonPropertyName("SiparisNo1")]
    public string? SiparisNo1 { get; set; }

    /// <summary>Siparis numarasi 2 (max 50)</summary>
    [JsonPropertyName("SiparisNo2")]
    public string? SiparisNo2 { get; set; }

    /// <summary>Departman (max 20)</summary>
    [JsonPropertyName("Departman")]
    public string? Departman { get; set; }

    /// <summary>Pozisyon numarasi (max 20)</summary>
    [JsonPropertyName("PozisyonNo")]
    public string? PozisyonNo { get; set; }

    /// <summary>Toplam brut agirlik (kg)</summary>
    [JsonPropertyName("Toplam_Brut_agirlik")]
    public decimal? ToplamBrutAgirlik { get; set; }

    /// <summary>Toplam net agirlik (kg)</summary>
    [JsonPropertyName("Toplam_Net_agirlik")]
    public decimal? ToplamNetAgirlik { get; set; }

    /// <summary>Referans tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Referans_tarihi")]
    public string? ReferansTarihi { get; set; }

    /// <summary>Firma bilgileri listesi (yurtIciMusteri, yurtDisiMusteri, nakliye, sorumlu)</summary>
    [JsonPropertyName("Firma_Bilgi")]
    public List<EvrimCreateFirma>? FirmaBilgi { get; set; }

    /// <summary>Damping kodu (max 20)</summary>
    [JsonPropertyName("DampingKodu")]
    public string? DampingKodu { get; set; }

    /// <summary>Ozel damping vergisi</summary>
    [JsonPropertyName("OzelDV")]
    public bool? OzelDV { get; set; }

    /// <summary>KKDF matrahi</summary>
    [JsonPropertyName("KKDFMatrah")]
    public decimal? KKDFMatrah { get; set; }

    /// <summary>KKDF vergi atma</summary>
    [JsonPropertyName("KKDFVergiAtma")]
    public bool? KKDFVergiAtma { get; set; }

    /// <summary>Vezne tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("VezneTarihi")]
    public string? VezneTarihi { get; set; }

    /// <summary>Vezne numarasi (max 20)</summary>
    [JsonPropertyName("VezneNo")]
    public string? VezneNo { get; set; }

    /// <summary>Musavir kodu (max 20)</summary>
    [JsonPropertyName("MusKod")]
    public string? MusKod { get; set; }

    /// <summary>Is kodu</summary>
    [JsonPropertyName("Is_kodu")]
    public string? IsKodu { get; set; }

    /// <summary>Is takip kodu. Orn: 01 (max 2)</summary>
    [JsonPropertyName("IsTakipKodu")]
    public string? IsTakipKodu { get; set; }

    /// <summary>Konsimento numarasi</summary>
    [JsonPropertyName("KonsimentoNo")]
    public string? KonsimentoNo { get; set; }

    /// <summary>Ozet beyanlar</summary>
    [JsonPropertyName("Ozetbeyanlar")]
    public object? Ozetbeyanlar { get; set; }

    /// <summary>Beyanname kalemleri (detay satirlari)</summary>
    [JsonPropertyName("Details")]
    public List<EvrimCreateDetail>? Details { get; set; }

    /// <summary>Dosya notlari</summary>
    [JsonPropertyName("MasterNotes")]
    public object? MasterNotes { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  YURT ICI HARCAMALAR
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Yurt ici harcamalar bilgileri (ardiye, banka komisyonu, KKDF vb.)
/// </summary>
public class EvrimCreateYurtIciHarcamalar
{
    /// <summary>Ardiye ucreti</summary>
    [JsonPropertyName("Ardiye")]
    public decimal? Ardiye { get; set; }

    /// <summary>Pul / Ordino / KF tutari</summary>
    [JsonPropertyName("Pul_Ord_KF")]
    public decimal? PulOrdKF { get; set; }

    /// <summary>Banka komisyonu</summary>
    [JsonPropertyName("BankaKom")]
    public decimal? BankaKom { get; set; }

    /// <summary>Diger harcama 1</summary>
    [JsonPropertyName("Diger1")]
    public decimal? Diger1 { get; set; }

    /// <summary>Diger harcama 2</summary>
    [JsonPropertyName("Diger2")]
    public decimal? Diger2 { get; set; }

    /// <summary>KKDF (Kaynak Kullanimi Destekleme Fonu)</summary>
    [JsonPropertyName("KKDF")]
    public decimal? KKDF { get; set; }

    /// <summary>Kultur fonu tutari</summary>
    [JsonPropertyName("Kultur_Fonu")]
    public decimal? KulturFonu { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  FIRMA BILGI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Firma bilgileri. Tip alanlari: yurtIciMusteri, yurtDisiMusteri, nakliye, sorumlu
/// </summary>
public class EvrimCreateFirma
{
    /// <summary>Firma tipi (yurtIciMusteri, yurtDisiMusteri, nakliye, sorumlu)</summary>
    [JsonPropertyName("Tip")]
    public string? Tip { get; set; }

    /// <summary>Firma numarasi</summary>
    [JsonPropertyName("No")]
    public int? No { get; set; }

    /// <summary>Hesap numarasi</summary>
    [JsonPropertyName("Account_No")]
    public string? AccountNo { get; set; }

    /// <summary>Vergi Kimlik Numarasi</summary>
    [JsonPropertyName("VKN")]
    public string? VKN { get; set; }

    /// <summary>YFKSIIKS (yurt disi firma iliskili kurum kodu)</summary>
    [JsonPropertyName("YFKSIIKS")]
    public string? YFKSIIKS { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  DETAIL (KALEM)
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Beyanname kalem detayi (GTIP, agirlik, fiyat, vergiler, dokumanlar vb.)
/// </summary>
public class EvrimCreateDetail
{
    /// <summary>Detay/kalem numarasi</summary>
    [JsonPropertyName("Detay_No")]
    public int? DetayNo { get; set; }

    /// <summary>GTIP kodu (max 12)</summary>
    [JsonPropertyName("Gtip")]
    public string? Gtip { get; set; }

    /// <summary>Mensei ulke kodu (max 4)</summary>
    [JsonPropertyName("ItemCountryOfOriginCode")]
    public string? ItemCountryOfOriginCode { get; set; }

    /// <summary>Brut agirlik (kg)</summary>
    [JsonPropertyName("GrossWeight")]
    public decimal? GrossWeight { get; set; }

    /// <summary>Net agirlik (kg)</summary>
    [JsonPropertyName("NetWeight")]
    public decimal? NetWeight { get; set; }

    /// <summary>Olcu birimi (max 4)</summary>
    [JsonPropertyName("UnitOfMeasure")]
    public string? UnitOfMeasure { get; set; }

    /// <summary>Olcu miktari</summary>
    [JsonPropertyName("AmountOfMeasure")]
    public decimal? AmountOfMeasure { get; set; }

    /// <summary>Uluslararasi anlasma kodu (max 6)</summary>
    [JsonPropertyName("Uluslararasi_anlasma")]
    public string? UluslararasiAnlasma { get; set; }

    /// <summary>Algilama birimi 1 (max 4)</summary>
    [JsonPropertyName("Algilama_birimi_1")]
    public string? AlgilamaBirimi1 { get; set; }

    /// <summary>Algilama miktari 1</summary>
    [JsonPropertyName("Algilama_miktari_1")]
    public decimal? AlgilamaMiktari1 { get; set; }

    /// <summary>Algilama birimi 2 (max 4)</summary>
    [JsonPropertyName("Algilama_birimi_2")]
    public string? AlgilamaBirimi2 { get; set; }

    /// <summary>Algilama miktari 2</summary>
    [JsonPropertyName("Algilama_miktari_2")]
    public decimal? AlgilamaMiktari2 { get; set; }

    /// <summary>Muafiyet kodu 1 (max 6)</summary>
    [JsonPropertyName("Muafiyetler_1")]
    public string? Muafiyetler1 { get; set; }

    /// <summary>Muafiyet kodu 2 (max 6)</summary>
    [JsonPropertyName("Muafiyetler_2")]
    public string? Muafiyetler2 { get; set; }

    /// <summary>Muafiyet kodu 3 (max 6)</summary>
    [JsonPropertyName("Muafiyetler_3")]
    public string? Muafiyetler3 { get; set; }

    /// <summary>Algilama birimi 3 (max 4)</summary>
    [JsonPropertyName("Algilama_birimi_3")]
    public string? AlgilamaBirimi3 { get; set; }

    /// <summary>Algilama miktari 3</summary>
    [JsonPropertyName("Algilama_miktari_3")]
    public decimal? AlgilamaMiktari3 { get; set; }

    /// <summary>Ek kod (max 4)</summary>
    [JsonPropertyName("Ek_kod")]
    public string? EkKod { get; set; }

    /// <summary>Ozellik kodu (max 4)</summary>
    [JsonPropertyName("Ozellik")]
    public string? Ozellik { get; set; }

    /// <summary>Toplam fatura tutari</summary>
    [JsonPropertyName("TotalAmount")]
    public decimal? TotalAmount { get; set; }

    /// <summary>Navlun miktari</summary>
    [JsonPropertyName("Navlun_miktari")]
    public decimal? NavlunMiktari { get; set; }

    /// <summary>Sigorta miktari</summary>
    [JsonPropertyName("Sigorta_miktari")]
    public decimal? SigortaMiktari { get; set; }

    /// <summary>Iskonto tutari</summary>
    [JsonPropertyName("DiscountAmount")]
    public decimal? DiscountAmount { get; set; }

    /// <summary>Esya ana referansi (max 50)</summary>
    [JsonPropertyName("ItemMaster")]
    public string? ItemMaster { get; set; }

    /// <summary>Uretici firma numarasi</summary>
    [JsonPropertyName("UreticiFirmaNo")]
    public int? UreticiFirmaNo { get; set; }

    /// <summary>Kap cinsi (max 4)</summary>
    [JsonPropertyName("Cinsi")]
    public string? Cinsi { get; set; }

    /// <summary>Kap adedi (max 10)</summary>
    [JsonPropertyName("PackageAmount")]
    public string? PackageAmount { get; set; }

    /// <summary>Miktar birimi (max 4)</summary>
    [JsonPropertyName("Miktar_birimi")]
    public string? MiktarBirimi { get; set; }

    /// <summary>Ikincil islem kodu (max 2)</summary>
    [JsonPropertyName("IkincilIslem")]
    public string? IkincilIslem { get; set; }

    /// <summary>Satir numarasi</summary>
    [JsonPropertyName("Satir_no")]
    public string? SatirNo { get; set; }

    /// <summary>Esya miktari</summary>
    [JsonPropertyName("ItemQuantity")]
    public decimal? ItemQuantity { get; set; }

    /// <summary>KDV orani (%)</summary>
    [JsonPropertyName("Kdv_orani")]
    public string? KdvOrani { get; set; }

    /// <summary>Kullanilmis esya (E/H)</summary>
    [JsonPropertyName("Kullanilmis_esya")]
    public string? KullanilmisEsya { get; set; }

    /// <summary>44 nolu han aciklamasi (max 250)</summary>
    [JsonPropertyName("Aciklama_44")]
    public string? Aciklama44 { get; set; }

    /// <summary>Yurt disi hesaplama yontemi</summary>
    [JsonPropertyName("YDHesaplama")]
    public string? YDHesaplama { get; set; }

    /// <summary>Birim fiyat</summary>
    [JsonPropertyName("Birim_fiyat")]
    public decimal? BirimFiyat { get; set; }

    /// <summary>OTV orani (%)</summary>
    [JsonPropertyName("OTV_orani")]
    public decimal? OTVOrani { get; set; }

    /// <summary>EMY (Ek Mali Yukumluluk) yok mu?</summary>
    [JsonPropertyName("EMY_Yok")]
    public bool? EMYYok { get; set; }

    /// <summary>ADV (Anti-Damping Vergisi) yok mu?</summary>
    [JsonPropertyName("ADV_Yok")]
    public bool? ADVYok { get; set; }

    /// <summary>EMY degistirme</summary>
    [JsonPropertyName("EMYDegistirme")]
    public bool? EMYDegistirme { get; set; }

    /// <summary>KKDF hesapla</summary>
    [JsonPropertyName("KKDFHesapla")]
    public bool? KKDFHesapla { get; set; }

    /// <summary>Kalem vergi listesi</summary>
    [JsonPropertyName("Vergiler")]
    public List<EvrimCreateVergi>? Vergiler { get; set; }

    /// <summary>Gumruk kategori kodu (max 10)</summary>
    [JsonPropertyName("G_Kategori_Kod")]
    public string? GKategoriKod { get; set; }

    /// <summary>Gumruk kategori alt kodu (max 10)</summary>
    [JsonPropertyName("G_Kategori_AltKod")]
    public string? GKategoriAltKod { get; set; }

    /// <summary>Damping kodu (max 20)</summary>
    [JsonPropertyName("DampingKodu")]
    public string? DampingKodu { get; set; }

    /// <summary>Doviz cinsi (max 3)</summary>
    [JsonPropertyName("Doviz")]
    public string? Doviz { get; set; }

    /// <summary>Kalem islem niteligi (max 3)</summary>
    [JsonPropertyName("Kalem_Islem_Niteligi")]
    public string? KalemIslemNiteligi { get; set; }

    /// <summary>Giris/cikis amaci kodu (max 3)</summary>
    [JsonPropertyName("Giris_Cikis_Amaci")]
    public string? GirisCikisAmaci { get; set; }

    /// <summary>Giris/cikis amaci aciklamasi (max 100)</summary>
    [JsonPropertyName("Giris_Cikis_Amaci_Aciklama")]
    public string? GirisCikisAmaciAciklama { get; set; }

    /// <summary>Dosya sira numarasi</summary>
    [JsonPropertyName("DosyaSiraNo")]
    public int? DosyaSiraNo { get; set; }

    /// <summary>Fatura numarasi (max 50)</summary>
    [JsonPropertyName("FaturaNo")]
    public string? FaturaNo { get; set; }

    /// <summary>Fatura tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Fatura_Tarihi")]
    public string? FaturaTarihi { get; set; }

    /// <summary>Satir GUID (benzersiz kimlik)</summary>
    [JsonPropertyName("SatirGuid")]
    public string? SatirGuid { get; set; }

    /// <summary>Malzeme kodu (max 50)</summary>
    [JsonPropertyName("Malzeme_Kodu")]
    public string? MalzemeKodu { get; set; }

    /// <summary>Masraf merkezi (max 50)</summary>
    [JsonPropertyName("MasrafMerkezi")]
    public string? MasrafMerkezi { get; set; }

    /// <summary>IGV (Ilave Gumruk Vergisi) yok mu?</summary>
    [JsonPropertyName("IGV_Yok")]
    public bool? IGVYok { get; set; }

    /// <summary>SAP siparis numarasi (max 50)</summary>
    [JsonPropertyName("SAP_ORDER_NO")]
    public string? SapOrderNo { get; set; }

    /// <summary>Satin alma siparis numarasi (max 50)</summary>
    [JsonPropertyName("PURCHASE_ORDER_NUMBER")]
    public string? PurchaseOrderNumber { get; set; }

    /// <summary>Sevkiyat referansi (max 50)</summary>
    [JsonPropertyName("SHIP_REFERENCE")]
    public string? ShipReference { get; set; }

    /// <summary>Urun numarasi (max 50)</summary>
    [JsonPropertyName("PRODUCT_NO")]
    public string? ProductNo { get; set; }

    /// <summary>Urun aciklamasi (max 250)</summary>
    [JsonPropertyName("PRODUCT_DESCRIPTION")]
    public string? ProductDescription { get; set; }

    /// <summary>ECCN (Export Control Classification Number)</summary>
    [JsonPropertyName("ECCN")]
    public string? ECCN { get; set; }

    /// <summary>Kutu numarasi (max 50)</summary>
    [JsonPropertyName("BOX_NO")]
    public string? BoxNo { get; set; }

    /// <summary>Siparis numarasi (max 50)</summary>
    [JsonPropertyName("ORDER_NUMBER")]
    public string? OrderNumber { get; set; }

    /// <summary>Alici bilgisi (max 100)</summary>
    [JsonPropertyName("CONSIGNEE")]
    public string? Consignee { get; set; }

    /// <summary>Belge 5 durumu</summary>
    [JsonPropertyName("Belge5Durum")]
    public string? Belge5Durum { get; set; }

    /// <summary>Belge 1 referans numarasi</summary>
    [JsonPropertyName("Belge1RefNo")]
    public string? Belge1RefNo { get; set; }

    /// <summary>Marka adi (max 100)</summary>
    [JsonPropertyName("Brand")]
    public string? Brand { get; set; }

    /// <summary>Seri/Sasi numarasi (max 100)</summary>
    [JsonPropertyName("Numara")]
    public string? Numara { get; set; }

    /// <summary>Detay notlari</summary>
    [JsonPropertyName("DetailNotes")]
    public object? DetailNotes { get; set; }

    /// <summary>Marka ve model bilgileri listesi</summary>
    [JsonPropertyName("Marka_model_bilgi")]
    public List<EvrimCreateMarkaModel>? MarkaModelBilgi { get; set; }

    /// <summary>Odeme sekilleri listesi</summary>
    [JsonPropertyName("OdemeSekilleri")]
    public List<EvrimCreateOdemeSekli>? OdemeSekilleri { get; set; }

    /// <summary>TCGB acma/kapatma bilgileri listesi</summary>
    [JsonPropertyName("Tcgbacmakapatma_Bilgi")]
    public List<EvrimCreateTcgbKapatma>? TcgbKapatmaBilgi { get; set; }

    /// <summary>Dokumanlar listesi</summary>
    [JsonPropertyName("Dokumanlar")]
    public List<EvrimCreateDokuman>? Dokumanlar { get; set; }

    /// <summary>Is takip kayitlari listesi</summary>
    [JsonPropertyName("IsTakipler")]
    public List<EvrimCreateIsTakip>? IsTakipler { get; set; }

    /// <summary>Konteyner bilgileri listesi</summary>
    [JsonPropertyName("Konteyner_Bilgi")]
    public List<EvrimCreateKonteyner>? KonteynerBilgi { get; set; }

    /// <summary>Royalti tutari</summary>
    [JsonPropertyName("Royalti")]
    public decimal? Royalti { get; set; }

    /// <summary>Esya tanimi 1 (max 250)</summary>
    [JsonPropertyName("EsyaTanimi1")]
    public string? EsyaTanimi1 { get; set; }

    /// <summary>Esya tanimi 2 (max 250)</summary>
    [JsonPropertyName("EsyaTanimi2")]
    public string? EsyaTanimi2 { get; set; }

    /// <summary>Esya tanimi 3 (max 250)</summary>
    [JsonPropertyName("EsyaTanimi3")]
    public string? EsyaTanimi3 { get; set; }

    /// <summary>Siparis numarasi (max 50)</summary>
    [JsonPropertyName("Order_No")]
    public string? OrderNo { get; set; }

    /// <summary>Yurt disi royalti tutari</summary>
    [JsonPropertyName("YurtDisi_Royalti")]
    public decimal? YurtDisiRoyalti { get; set; }

    /// <summary>Yurt disi royalti dovizi (max 3)</summary>
    [JsonPropertyName("YurtDisi_Royalti_Dovizi")]
    public string? YurtDisiRoyaltiDovizi { get; set; }

    /// <summary>Yurt disi banka masraflari</summary>
    [JsonPropertyName("YurtDisi_Banka")]
    public decimal? YurtDisiBanka { get; set; }

    /// <summary>Yurt disi komisyon</summary>
    [JsonPropertyName("YurtDisi_Komisyon")]
    public decimal? YurtDisiKomisyon { get; set; }

    /// <summary>Yurt disi depolama</summary>
    [JsonPropertyName("YurtDisi_Depolama")]
    public decimal? YurtDisiDepolama { get; set; }

    /// <summary>Toplam yurt disi harcamalar</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalar")]
    public decimal? ToplamYurtDisiHarcamalar { get; set; }

    /// <summary>Yurt disi harcamalar dovizi (max 3)</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_dovizi")]
    public string? ToplamYurtDisiHarcamalarinDovizi { get; set; }

    /// <summary>Yurt disi harcamalar aciklama (max 10)</summary>
    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_aciklama")]
    public string? ToplamYurtDisiHarcamalarinAciklama { get; set; }

    /// <summary>Kalem yurt ici harcamalar detayi</summary>
    [JsonPropertyName("Yurt_ici_harcamalar")]
    public EvrimCreateYurtIciHarcamalar? DetailYurtIciHarcamalar { get; set; }

    /// <summary>Siparis turu</summary>
    [JsonPropertyName("siparisTuru")]
    public string? SiparisTuru { get; set; }

    /// <summary>Istatistiki kiymet</summary>
    [JsonPropertyName("Ist_Kiymet")]
    public decimal? IstKiymet { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  VERGI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Vergi bilgisi (GV, KDV, OTV, KKDF vb.)
/// </summary>
public class EvrimCreateVergi
{
    /// <summary>Vergi sira numarasi</summary>
    [JsonPropertyName("Sira_No")]
    public int? SiraNo { get; set; }

    /// <summary>Vergi turu kodu (Orn: GV, KDV, OTV)</summary>
    [JsonPropertyName("VTuru")]
    public string? VTuru { get; set; }

    /// <summary>Vergi matrahi</summary>
    [JsonPropertyName("VVergi_Matrahi")]
    public decimal? VVergiMatrahi { get; set; }

    /// <summary>Vergi orani (%)</summary>
    [JsonPropertyName("VOrani")]
    public string? VOrani { get; set; }

    /// <summary>Vergi tutari</summary>
    [JsonPropertyName("VTutari")]
    public string? VTutari { get; set; }

    /// <summary>Vergi odeme sekli</summary>
    [JsonPropertyName("VOS")]
    public string? VOS { get; set; }

    /// <summary>Vergi vadeli/yevmiye</summary>
    [JsonPropertyName("VergiVY")]
    public string? VergiVY { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  MARKA MODEL BILGI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Marka/model bilgisi (arac, elektronik esya vb. icin detayli bilgiler)
/// </summary>
public class EvrimCreateMarkaModel
{
    /// <summary>Kalem fiyati</summary>
    [JsonPropertyName("Kalem_Fiyat")]
    public decimal? KalemFiyat { get; set; }

    /// <summary>Miktar</summary>
    [JsonPropertyName("Miktar")]
    public decimal? Miktar { get; set; }

    /// <summary>Brut agirlik (kg)</summary>
    [JsonPropertyName("BrutKG")]
    public decimal? BrutKG { get; set; }

    /// <summary>Net agirlik (kg)</summary>
    [JsonPropertyName("NetKG")]
    public decimal? NetKG { get; set; }

    /// <summary>Marka turu (max 4)</summary>
    [JsonPropertyName("Marka_Turu")]
    public string? MarkaTuru { get; set; }

    /// <summary>Marka tescil numarasi (max 50)</summary>
    [JsonPropertyName("Marka_Tescil_No")]
    public string? MarkaTescilNo { get; set; }

    /// <summary>Marka adi (max 100)</summary>
    [JsonPropertyName("Marka_Adi")]
    public string? MarkaAdi { get; set; }

    /// <summary>Referans numarasi (max 50)</summary>
    [JsonPropertyName("Referans_No")]
    public string? ReferansNo { get; set; }

    /// <summary>Model yili (max 4)</summary>
    [JsonPropertyName("Model_Yili")]
    public string? ModelYili { get; set; }

    /// <summary>Model (max 100)</summary>
    [JsonPropertyName("Model")]
    public string? Model { get; set; }

    /// <summary>Motor hacmi (cc)</summary>
    [JsonPropertyName("Motor_Hacmi")]
    public string? MotorHacmi { get; set; }

    /// <summary>Silindir adedi</summary>
    [JsonPropertyName("Silindir_Adedi")]
    public string? SilindirAdedi { get; set; }

    /// <summary>Renk (max 50)</summary>
    [JsonPropertyName("Renk")]
    public string? Renk { get; set; }

    /// <summary>Marka satir ID</summary>
    [JsonPropertyName("MlineID")]
    public string? MlineID { get; set; }

    /// <summary>TSE belge numarasi (max 50)</summary>
    [JsonPropertyName("TSE_No")]
    public string? TSENo { get; set; }

    /// <summary>Malin cinsi (max 100)</summary>
    [JsonPropertyName("Malin_Cinsi")]
    public string? MalinCinsi { get; set; }

    /// <summary>Motor numarasi (max 50)</summary>
    [JsonPropertyName("Motor_No")]
    public string? MotorNo { get; set; }

    /// <summary>IMEI numarasi (max 20)</summary>
    [JsonPropertyName("IMEI")]
    public string? IMEI { get; set; }

    /// <summary>Siparis numarasi (max 50)</summary>
    [JsonPropertyName("Order_No")]
    public string? OrderNo { get; set; }

    /// <summary>DTS dosya numarasi (max 50)</summary>
    [JsonPropertyName("DtsDosyaNo")]
    public string? DtsDosyaNo { get; set; }

    /// <summary>Fatura numarasi (max 50)</summary>
    [JsonPropertyName("FaturaNo")]
    public string? FaturaNo { get; set; }

    /// <summary>Fatura tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("FaturaTarihi")]
    public string? FaturaTarihi { get; set; }

    /// <summary>Ticari tanim (max 250)</summary>
    [JsonPropertyName("TicariTanim")]
    public string? TicariTanim { get; set; }

    /// <summary>Vites turu (max 20)</summary>
    [JsonPropertyName("VitesTuru")]
    public string? VitesTuru { get; set; }

    /// <summary>Motor tipi (max 20)</summary>
    [JsonPropertyName("MotorTipi")]
    public string? MotorTipi { get; set; }

    /// <summary>Motor gucu (kW)</summary>
    [JsonPropertyName("MotorGucu")]
    public decimal? MotorGucu { get; set; }

    /// <summary>Aktarma alani 1 (max 100)</summary>
    [JsonPropertyName("Aktar1")]
    public string? Aktar1 { get; set; }

    /// <summary>Aktarma alani 2 (max 100)</summary>
    [JsonPropertyName("Aktar2")]
    public string? Aktar2 { get; set; }

    /// <summary>Aktarma alani 3 (max 100)</summary>
    [JsonPropertyName("Aktar3")]
    public string? Aktar3 { get; set; }

    /// <summary>Aktarma alani 4 (max 100)</summary>
    [JsonPropertyName("Aktar4")]
    public string? Aktar4 { get; set; }

    /// <summary>Aktarma alani 5 (max 100)</summary>
    [JsonPropertyName("Aktar5")]
    public string? Aktar5 { get; set; }

    /// <summary>Aktarma alani 6 (max 100)</summary>
    [JsonPropertyName("Aktar6")]
    public string? Aktar6 { get; set; }

    /// <summary>Aktarma alani 7 (max 100)</summary>
    [JsonPropertyName("Aktar7")]
    public string? Aktar7 { get; set; }

    /// <summary>Aktarma alani 8 (max 100)</summary>
    [JsonPropertyName("Aktar8")]
    public string? Aktar8 { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  ODEME SEKLI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Odeme sekli bilgisi (doviz transferi, akreditif vb.)
/// </summary>
public class EvrimCreateOdemeSekli
{
    /// <summary>Odeme sekli kodu</summary>
    [JsonPropertyName("OdemeSekliKodu")]
    public int? OdemeSekliKodu { get; set; }

    /// <summary>Odeme tutari</summary>
    [JsonPropertyName("OdemeTutari")]
    public decimal? OdemeTutari { get; set; }

    /// <summary>Ithalat akreditif belge kodu (max 30)</summary>
    [JsonPropertyName("ImportLetterOfCreditDocumentCode")]
    public string? ImportLetterOfCreditDocumentCode { get; set; }

    /// <summary>TBF (Transfer Bildirim Formu) tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("TBFTarihi")]
    public string? TBFTarihi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  TCGB KAPATMA BILGI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// TCGB (Tek Pencere Gumruk Beyannamesi) acma/kapatma bilgisi
/// </summary>
public class EvrimCreateTcgbKapatma
{
    /// <summary>Antrepo beyanname numarasi (max 30)</summary>
    [JsonPropertyName("BWDeclarationNo")]
    public string? BWDeclarationNo { get; set; }

    /// <summary>Kapatilan miktar</summary>
    [JsonPropertyName("Kapatilan_miktar")]
    public decimal? KapatilanMiktar { get; set; }

    /// <summary>Satir numarasi</summary>
    [JsonPropertyName("LineNo")]
    public string? LineNo { get; set; }

    /// <summary>Aciklama (max 250)</summary>
    [JsonPropertyName("Aciklama")]
    public string? Aciklama { get; set; }

    /// <summary>Antrepo beyanname tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("BWDeclarationDate")]
    public string? BWDeclarationDate { get; set; }

    /// <summary>Kapatilan kap adedi</summary>
    [JsonPropertyName("Kapatilan_kap_adedi")]
    public decimal? KapatilanKapAdedi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  DOKUMAN
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Dokuman/belge bilgisi (fatura, mensei, ATR, EUR1 vb.)
/// </summary>
public class EvrimCreateDokuman
{
    /// <summary>Belge kodu (max 10)</summary>
    [JsonPropertyName("Kod")]
    public string? Kod { get; set; }

    /// <summary>Belge tipi (max 4)</summary>
    [JsonPropertyName("Tip")]
    public string? Tip { get; set; }

    /// <summary>Arti bilgisi</summary>
    [JsonPropertyName("Arti")]
    public string? Arti { get; set; }

    /// <summary>Dogrulama bilgisi</summary>
    [JsonPropertyName("Dogrulama")]
    public string? Dogrulama { get; set; }

    /// <summary>Belge tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Belge_tarihi")]
    public string? BelgeTarihi { get; set; }

    /// <summary>Belge referans numarasi (max 50)</summary>
    [JsonPropertyName("Referans")]
    public string? Referans { get; set; }

    /// <summary>Cikis sira numarasi</summary>
    [JsonPropertyName("Cikis_sira_no")]
    public int? CikisSiraNo { get; set; }

    /// <summary>Vize tarihi (yyyy-MM-dd)</summary>
    [JsonPropertyName("Vize_tarihi")]
    public string? VizeTarihi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  IS TAKIP
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Is takip kaydi (islem adimlari ve sorumlulari)
/// </summary>
public class EvrimCreateIsTakip
{
    /// <summary>Is takip kodu (max 2)</summary>
    [JsonPropertyName("Kod")]
    public string? Kod { get; set; }

    /// <summary>Aciklama (max 250)</summary>
    [JsonPropertyName("Aciklama")]
    public string? Aciklama { get; set; }

    /// <summary>Tarih ve saat (yyyy-MM-ddTHH:mm:ss)</summary>
    [JsonPropertyName("TarihSaat")]
    public string? TarihSaat { get; set; }

    /// <summary>Kullanici kodu (max 20)</summary>
    [JsonPropertyName("KulKod")]
    public string? KulKod { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  KONTEYNER BILGI
// ════════════════════════════════════════════════════════════════
/// <summary>
/// Konteyner bilgisi
/// </summary>
public class EvrimCreateKonteyner
{
    /// <summary>Konteyner numarasi (max 20)</summary>
    [JsonPropertyName("Konteyner_No")]
    public string? KonteynerNo { get; set; }

    /// <summary>Ulke kodu (max 4)</summary>
    [JsonPropertyName("Ulke_Kodu")]
    public string? UlkeKodu { get; set; }

    /// <summary>Konteyner tipi (max 4)</summary>
    [JsonPropertyName("KonteynerTipi")]
    public string? KonteynerTipi { get; set; }
}
