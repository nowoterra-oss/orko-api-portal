using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim yeni API - POST /api/create_export_declaration ve /api/create_import_declaration icin kullanilir.
/// Alan adlari Evrim'in yeni swagger semasina birebir uyar.
/// </summary>
public class EvrimCreateDeclarationRequest
{
    [JsonPropertyName("dosyaTipi")]
    public string? DosyaTipi { get; set; }

    [JsonPropertyName("RefNo")]
    public string? RefNo { get; set; }

    [JsonPropertyName("Created_user")]
    public string? CreatedUser { get; set; }

    [JsonPropertyName("File_date")]
    public string? FileDate { get; set; }

    [JsonPropertyName("Ihracat")]
    public bool? Ihracat { get; set; }

    [JsonPropertyName("RegimeCode")]
    public string? RegimeCode { get; set; }

    [JsonPropertyName("Gumruk")]
    public int? Gumruk { get; set; }

    [JsonPropertyName("Basitlestirilmis_usul")]
    public string? BasitlestirilmisUsul { get; set; }

    [JsonPropertyName("Yuk_belgeleri_sayisi")]
    public int? YukBelgeleriSayisi { get; set; }

    [JsonPropertyName("ToCountryCode")]
    public string? ToCountryCode { get; set; }

    [JsonPropertyName("ExitCountryCode")]
    public string? ExitCountryCode { get; set; }

    [JsonPropertyName("ExportFromCountryCode")]
    public string? ExportFromCountryCode { get; set; }

    [JsonPropertyName("InternalVehicleTypeCode")]
    public string? InternalVehicleTypeCode { get; set; }

    [JsonPropertyName("Cikistaki_aracin_ulkesi")]
    public string? CikistakiAracinUlkesi { get; set; }

    [JsonPropertyName("IncotermCode")]
    public string? IncotermCode { get; set; }

    [JsonPropertyName("Konteyner")]
    public string? Konteyner { get; set; }

    [JsonPropertyName("Sinirdaki_aracin_tipi")]
    public string? SinirdakiAracinTipi { get; set; }

    [JsonPropertyName("BorderVehicle")]
    public string? BorderVehicle { get; set; }

    [JsonPropertyName("Sinirdaki_aracin_ulkesi")]
    public string? SinirdakiAracinUlkesi { get; set; }

    [JsonPropertyName("Toplam_fatura")]
    public decimal? ToplamFatura { get; set; }

    [JsonPropertyName("CurrencyTypeCode")]
    public string? CurrencyTypeCode { get; set; }

    [JsonPropertyName("TahminiVarisTarihi")]
    public string? TahminiVarisTarihi { get; set; }

    [JsonPropertyName("Freight")]
    public decimal? Freight { get; set; }

    [JsonPropertyName("FreightCurrencyType")]
    public string? FreightCurrencyType { get; set; }

    [JsonPropertyName("BorderVehicleTypeCode")]
    public string? BorderVehicleTypeCode { get; set; }

    [JsonPropertyName("Alici_satici_iliskisi")]
    public int? AliciSaticiIliskisi { get; set; }

    [JsonPropertyName("Insurance")]
    public decimal? Insurance { get; set; }

    [JsonPropertyName("InsuranceCurrencyType")]
    public string? InsuranceCurrencyType { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalar")]
    public decimal? ToplamYurtDisiHarcamalar { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_dovizi")]
    public string? ToplamYurtDisiHarcamalarinDovizi { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_aciklama")]
    public string? ToplamYurtDisiHarcamalarinAciklama { get; set; }

    [JsonPropertyName("Teslim_yeri")]
    public string? TeslimYeri { get; set; }

    [JsonPropertyName("Cikistaki_aracin_kimligi")]
    public string? CikistakiAracinKimligi { get; set; }

    [JsonPropertyName("Kap_adedi")]
    public string? KapAdedi { get; set; }

    [JsonPropertyName("Yukleme_bosaltma_yeri")]
    public string? YuklemeBosaltmaYeri { get; set; }

    [JsonPropertyName("DetayAcik2")]
    public string? DetayAcik2 { get; set; }

    [JsonPropertyName("YurtDisi_Royalti")]
    public decimal? YurtDisiRoyalti { get; set; }

    [JsonPropertyName("YurtDisi_Royalti_Dovizi")]
    public string? YurtDisiRoyaltiDovizi { get; set; }

    [JsonPropertyName("Ithal_Harci")]
    public decimal? IthalHarci { get; set; }

    [JsonPropertyName("YurtDisi_Banka")]
    public decimal? YurtDisiBanka { get; set; }

    [JsonPropertyName("YurtDisi_Komisyon")]
    public decimal? YurtDisiKomisyon { get; set; }

    [JsonPropertyName("YurtDisi_Depolama")]
    public decimal? YurtDisiDepolama { get; set; }

    [JsonPropertyName("Yurt_ici_harcamalar")]
    public EvrimCreateYurtIciHarcamalar? YurtIciHarcamalar { get; set; }

    [JsonPropertyName("PaymentTypeCode")]
    public int? PaymentTypeCode { get; set; }

    [JsonPropertyName("BankCode")]
    public string? BankCode { get; set; }

    [JsonPropertyName("BondedWarehouseFirmCode")]
    public string? BondedWarehouseFirmCode { get; set; }

    [JsonPropertyName("ImportCustomsCode")]
    public int? ImportCustomsCode { get; set; }

    [JsonPropertyName("BWTo")]
    public string? BWTo { get; set; }

    [JsonPropertyName("Esyanin_bulundugu_yer")]
    public string? EsyaninBulunduguYer { get; set; }

    [JsonPropertyName("LimanKodu")]
    public string? LimanKodu { get; set; }

    [JsonPropertyName("DestinationCustomsCode")]
    public int? DestinationCustomsCode { get; set; }

    [JsonPropertyName("Islemin_niteligi")]
    public string? IsleminNiteligi { get; set; }

    [JsonPropertyName("Aciklamalar")]
    public string? Aciklamalar { get; set; }

    [JsonPropertyName("Kullanici_kodu")]
    public int? KullaniciKodu { get; set; }

    [JsonPropertyName("BirlikReferans")]
    public string? BirlikReferans { get; set; }

    [JsonPropertyName("TirKntynrSayisi")]
    public int? TirKntynrSayisi { get; set; }

    [JsonPropertyName("SigortaYuzde")]
    public decimal? SigortaYuzde { get; set; }

    [JsonPropertyName("NavlunYuzde")]
    public decimal? NavlunYuzde { get; set; }

    [JsonPropertyName("TrnsitGA")]
    public string? TrnsitGA { get; set; }

    [JsonPropertyName("Tasarlanan_guzergah")]
    public string? TasarlananGuzergah { get; set; }

    [JsonPropertyName("Tasarlanan_guzergah2")]
    public string? TasarlananGuzergah2 { get; set; }

    [JsonPropertyName("Tasarlanan_guzergah3")]
    public string? TasarlananGuzergah3 { get; set; }

    [JsonPropertyName("Tasarlanan_guzergah4")]
    public string? TasarlananGuzergah4 { get; set; }

    [JsonPropertyName("Teminat")]
    public string? Teminat { get; set; }

    [JsonPropertyName("TTip")]
    public string? TTip { get; set; }

    [JsonPropertyName("TDigerRefNo")]
    public string? TDigerRefNo { get; set; }

    [JsonPropertyName("TGlobalGarantiNo")]
    public string? TGlobalGarantiNo { get; set; }

    [JsonPropertyName("TYuzde")]
    public decimal? TYuzde { get; set; }

    [JsonPropertyName("TAciklama")]
    public string? TAciklama { get; set; }

    [JsonPropertyName("IndirimliTeminat")]
    public bool? IndirimliTeminat { get; set; }

    [JsonPropertyName("TOdeme")]
    public string? TOdeme { get; set; }

    [JsonPropertyName("SureSonuTarih")]
    public string? SureSonuTarih { get; set; }

    [JsonPropertyName("TahminiBaslangicTar")]
    public string? TahminiBaslangicTar { get; set; }

    [JsonPropertyName("TahminiBitisTarihi")]
    public string? TahminiBitisTarihi { get; set; }

    [JsonPropertyName("SiparisNo1")]
    public string? SiparisNo1 { get; set; }

    [JsonPropertyName("SiparisNo2")]
    public string? SiparisNo2 { get; set; }

    [JsonPropertyName("Departman")]
    public string? Departman { get; set; }

    [JsonPropertyName("PozisyonNo")]
    public string? PozisyonNo { get; set; }

    [JsonPropertyName("Toplam_Brut_agirlik")]
    public decimal? ToplamBrutAgirlik { get; set; }

    [JsonPropertyName("Toplam_Net_agirlik")]
    public decimal? ToplamNetAgirlik { get; set; }

    [JsonPropertyName("Referans_tarihi")]
    public string? ReferansTarihi { get; set; }

    [JsonPropertyName("Firma_Bilgi")]
    public List<EvrimCreateFirma>? FirmaBilgi { get; set; }

    [JsonPropertyName("DampingKodu")]
    public string? DampingKodu { get; set; }

    [JsonPropertyName("OzelDV")]
    public bool? OzelDV { get; set; }

    [JsonPropertyName("KKDFMatrah")]
    public decimal? KKDFMatrah { get; set; }

    [JsonPropertyName("KKDFVergiAtma")]
    public bool? KKDFVergiAtma { get; set; }

    [JsonPropertyName("VezneTarihi")]
    public string? VezneTarihi { get; set; }

    [JsonPropertyName("VezneNo")]
    public string? VezneNo { get; set; }

    [JsonPropertyName("MusKod")]
    public string? MusKod { get; set; }

    [JsonPropertyName("Is_kodu")]
    public string? IsKodu { get; set; }

    [JsonPropertyName("IsTakipKodu")]
    public string? IsTakipKodu { get; set; }

    [JsonPropertyName("KonsimentoNo")]
    public string? KonsimentoNo { get; set; }

    [JsonPropertyName("Ozetbeyanlar")]
    public object? Ozetbeyanlar { get; set; }

    [JsonPropertyName("Details")]
    public List<EvrimCreateDetail>? Details { get; set; }

    [JsonPropertyName("MasterNotes")]
    public object? MasterNotes { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  YURT ICI HARCAMALAR
// ════════════════════════════════════════════════════════════════
public class EvrimCreateYurtIciHarcamalar
{
    [JsonPropertyName("Ardiye")]
    public decimal? Ardiye { get; set; }

    [JsonPropertyName("Pul_Ord_KF")]
    public decimal? PulOrdKF { get; set; }

    [JsonPropertyName("BankaKom")]
    public decimal? BankaKom { get; set; }

    [JsonPropertyName("Diger1")]
    public decimal? Diger1 { get; set; }

    [JsonPropertyName("Diger2")]
    public decimal? Diger2 { get; set; }

    [JsonPropertyName("KKDF")]
    public decimal? KKDF { get; set; }

    [JsonPropertyName("Kultur_Fonu")]
    public decimal? KulturFonu { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  FIRMA BILGI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateFirma
{
    [JsonPropertyName("Tip")]
    public string? Tip { get; set; }

    [JsonPropertyName("No")]
    public int? No { get; set; }

    [JsonPropertyName("Account_No")]
    public string? AccountNo { get; set; }

    [JsonPropertyName("VKN")]
    public string? VKN { get; set; }

    [JsonPropertyName("YFKSIIKS")]
    public string? YFKSIIKS { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  DETAIL (KALEM)
// ════════════════════════════════════════════════════════════════
public class EvrimCreateDetail
{
    [JsonPropertyName("Detay_No")]
    public int? DetayNo { get; set; }

    [JsonPropertyName("Gtip")]
    public string? Gtip { get; set; }

    [JsonPropertyName("ItemCountryOfOriginCode")]
    public string? ItemCountryOfOriginCode { get; set; }

    [JsonPropertyName("GrossWeight")]
    public decimal? GrossWeight { get; set; }

    [JsonPropertyName("NetWeight")]
    public decimal? NetWeight { get; set; }

    [JsonPropertyName("UnitOfMeasure")]
    public string? UnitOfMeasure { get; set; }

    [JsonPropertyName("AmountOfMeasure")]
    public decimal? AmountOfMeasure { get; set; }

    [JsonPropertyName("Uluslararasi_anlasma")]
    public string? UluslararasiAnlasma { get; set; }

    [JsonPropertyName("Algilama_birimi_1")]
    public string? AlgilamaBirimi1 { get; set; }

    [JsonPropertyName("Algilama_miktari_1")]
    public decimal? AlgilamaMiktari1 { get; set; }

    [JsonPropertyName("Algilama_birimi_2")]
    public string? AlgilamaBirimi2 { get; set; }

    [JsonPropertyName("Algilama_miktari_2")]
    public decimal? AlgilamaMiktari2 { get; set; }

    [JsonPropertyName("Muafiyetler_1")]
    public string? Muafiyetler1 { get; set; }

    [JsonPropertyName("Muafiyetler_2")]
    public string? Muafiyetler2 { get; set; }

    [JsonPropertyName("Muafiyetler_3")]
    public string? Muafiyetler3 { get; set; }

    [JsonPropertyName("Algilama_birimi_3")]
    public string? AlgilamaBirimi3 { get; set; }

    [JsonPropertyName("Algilama_miktari_3")]
    public decimal? AlgilamaMiktari3 { get; set; }

    [JsonPropertyName("Ek_kod")]
    public string? EkKod { get; set; }

    [JsonPropertyName("Ozellik")]
    public string? Ozellik { get; set; }

    [JsonPropertyName("TotalAmount")]
    public decimal? TotalAmount { get; set; }

    [JsonPropertyName("Navlun_miktari")]
    public decimal? NavlunMiktari { get; set; }

    [JsonPropertyName("Sigorta_miktari")]
    public decimal? SigortaMiktari { get; set; }

    [JsonPropertyName("DiscountAmount")]
    public decimal? DiscountAmount { get; set; }

    [JsonPropertyName("ItemMaster")]
    public string? ItemMaster { get; set; }

    [JsonPropertyName("UreticiFirmaNo")]
    public int? UreticiFirmaNo { get; set; }

    [JsonPropertyName("Cinsi")]
    public string? Cinsi { get; set; }

    [JsonPropertyName("PackageAmount")]
    public string? PackageAmount { get; set; }

    [JsonPropertyName("Miktar_birimi")]
    public string? MiktarBirimi { get; set; }

    [JsonPropertyName("IkincilIslem")]
    public string? IkincilIslem { get; set; }

    [JsonPropertyName("Satir_no")]
    public string? SatirNo { get; set; }

    [JsonPropertyName("ItemQuantity")]
    public decimal? ItemQuantity { get; set; }

    [JsonPropertyName("Kdv_orani")]
    public string? KdvOrani { get; set; }

    [JsonPropertyName("Kullanilmis_esya")]
    public string? KullanilmisEsya { get; set; }

    [JsonPropertyName("Aciklama_44")]
    public string? Aciklama44 { get; set; }

    [JsonPropertyName("YDHesaplama")]
    public string? YDHesaplama { get; set; }

    [JsonPropertyName("Birim_fiyat")]
    public decimal? BirimFiyat { get; set; }

    [JsonPropertyName("OTV_orani")]
    public decimal? OTVOrani { get; set; }

    [JsonPropertyName("EMY_Yok")]
    public bool? EMYYok { get; set; }

    [JsonPropertyName("ADV_Yok")]
    public bool? ADVYok { get; set; }

    [JsonPropertyName("EMYDegistirme")]
    public bool? EMYDegistirme { get; set; }

    [JsonPropertyName("KKDFHesapla")]
    public bool? KKDFHesapla { get; set; }

    [JsonPropertyName("Vergiler")]
    public List<EvrimCreateVergi>? Vergiler { get; set; }

    [JsonPropertyName("G_Kategori_Kod")]
    public string? GKategoriKod { get; set; }

    [JsonPropertyName("G_Kategori_AltKod")]
    public string? GKategoriAltKod { get; set; }

    [JsonPropertyName("DampingKodu")]
    public string? DampingKodu { get; set; }

    [JsonPropertyName("Doviz")]
    public string? Doviz { get; set; }

    [JsonPropertyName("Kalem_Islem_Niteligi")]
    public string? KalemIslemNiteligi { get; set; }

    [JsonPropertyName("Giris_Cikis_Amaci")]
    public string? GirisCikisAmaci { get; set; }

    [JsonPropertyName("Giris_Cikis_Amaci_Aciklama")]
    public string? GirisCikisAmaciAciklama { get; set; }

    [JsonPropertyName("DosyaSiraNo")]
    public int? DosyaSiraNo { get; set; }

    [JsonPropertyName("FaturaNo")]
    public string? FaturaNo { get; set; }

    [JsonPropertyName("Fatura_Tarihi")]
    public string? FaturaTarihi { get; set; }

    [JsonPropertyName("SatirGuid")]
    public string? SatirGuid { get; set; }

    [JsonPropertyName("Malzeme_Kodu")]
    public string? MalzemeKodu { get; set; }

    [JsonPropertyName("MasrafMerkezi")]
    public string? MasrafMerkezi { get; set; }

    [JsonPropertyName("IGV_Yok")]
    public bool? IGVYok { get; set; }

    [JsonPropertyName("SAP_ORDER_NO")]
    public string? SapOrderNo { get; set; }

    [JsonPropertyName("PURCHASE_ORDER_NUMBER")]
    public string? PurchaseOrderNumber { get; set; }

    [JsonPropertyName("SHIP_REFERENCE")]
    public string? ShipReference { get; set; }

    [JsonPropertyName("PRODUCT_NO")]
    public string? ProductNo { get; set; }

    [JsonPropertyName("PRODUCT_DESCRIPTION")]
    public string? ProductDescription { get; set; }

    [JsonPropertyName("ECCN")]
    public string? ECCN { get; set; }

    [JsonPropertyName("BOX_NO")]
    public string? BoxNo { get; set; }

    [JsonPropertyName("ORDER_NUMBER")]
    public string? OrderNumber { get; set; }

    [JsonPropertyName("CONSIGNEE")]
    public string? Consignee { get; set; }

    [JsonPropertyName("Belge5Durum")]
    public string? Belge5Durum { get; set; }

    [JsonPropertyName("Belge1RefNo")]
    public string? Belge1RefNo { get; set; }

    [JsonPropertyName("Brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("Numara")]
    public string? Numara { get; set; }

    [JsonPropertyName("DetailNotes")]
    public object? DetailNotes { get; set; }

    [JsonPropertyName("Marka_model_bilgi")]
    public List<EvrimCreateMarkaModel>? MarkaModelBilgi { get; set; }

    [JsonPropertyName("OdemeSekilleri")]
    public List<EvrimCreateOdemeSekli>? OdemeSekilleri { get; set; }

    [JsonPropertyName("Tcgbacmakapatma_Bilgi")]
    public List<EvrimCreateTcgbKapatma>? TcgbKapatmaBilgi { get; set; }

    [JsonPropertyName("Dokumanlar")]
    public List<EvrimCreateDokuman>? Dokumanlar { get; set; }

    [JsonPropertyName("IsTakipler")]
    public List<EvrimCreateIsTakip>? IsTakipler { get; set; }

    [JsonPropertyName("Konteyner_Bilgi")]
    public List<EvrimCreateKonteyner>? KonteynerBilgi { get; set; }

    [JsonPropertyName("Royalti")]
    public decimal? Royalti { get; set; }

    [JsonPropertyName("EsyaTanimi1")]
    public string? EsyaTanimi1 { get; set; }

    [JsonPropertyName("EsyaTanimi2")]
    public string? EsyaTanimi2 { get; set; }

    [JsonPropertyName("EsyaTanimi3")]
    public string? EsyaTanimi3 { get; set; }

    [JsonPropertyName("Order_No")]
    public string? OrderNo { get; set; }

    [JsonPropertyName("YurtDisi_Royalti")]
    public decimal? YurtDisiRoyalti { get; set; }

    [JsonPropertyName("YurtDisi_Royalti_Dovizi")]
    public string? YurtDisiRoyaltiDovizi { get; set; }

    [JsonPropertyName("YurtDisi_Banka")]
    public decimal? YurtDisiBanka { get; set; }

    [JsonPropertyName("YurtDisi_Komisyon")]
    public decimal? YurtDisiKomisyon { get; set; }

    [JsonPropertyName("YurtDisi_Depolama")]
    public decimal? YurtDisiDepolama { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalar")]
    public decimal? ToplamYurtDisiHarcamalar { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_dovizi")]
    public string? ToplamYurtDisiHarcamalarinDovizi { get; set; }

    [JsonPropertyName("Toplam_yurt_disi_harcamalarin_aciklama")]
    public string? ToplamYurtDisiHarcamalarinAciklama { get; set; }

    [JsonPropertyName("Yurt_ici_harcamalar")]
    public EvrimCreateYurtIciHarcamalar? DetailYurtIciHarcamalar { get; set; }

    [JsonPropertyName("siparisTuru")]
    public string? SiparisTuru { get; set; }

    [JsonPropertyName("Ist_Kiymet")]
    public decimal? IstKiymet { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  VERGI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateVergi
{
    [JsonPropertyName("Sira_No")]
    public int? SiraNo { get; set; }

    [JsonPropertyName("VTuru")]
    public string? VTuru { get; set; }

    [JsonPropertyName("VVergi_Matrahi")]
    public decimal? VVergiMatrahi { get; set; }

    [JsonPropertyName("VOrani")]
    public string? VOrani { get; set; }

    [JsonPropertyName("VTutari")]
    public string? VTutari { get; set; }

    [JsonPropertyName("VOS")]
    public string? VOS { get; set; }

    [JsonPropertyName("VergiVY")]
    public string? VergiVY { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  MARKA MODEL BILGI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateMarkaModel
{
    [JsonPropertyName("Kalem_Fiyat")]
    public decimal? KalemFiyat { get; set; }

    [JsonPropertyName("Miktar")]
    public decimal? Miktar { get; set; }

    [JsonPropertyName("BrutKG")]
    public decimal? BrutKG { get; set; }

    [JsonPropertyName("NetKG")]
    public decimal? NetKG { get; set; }

    [JsonPropertyName("Marka_Turu")]
    public string? MarkaTuru { get; set; }

    [JsonPropertyName("Marka_Tescil_No")]
    public string? MarkaTescilNo { get; set; }

    [JsonPropertyName("Marka_Adi")]
    public string? MarkaAdi { get; set; }

    [JsonPropertyName("Referans_No")]
    public string? ReferansNo { get; set; }

    [JsonPropertyName("Model_Yili")]
    public string? ModelYili { get; set; }

    [JsonPropertyName("Model")]
    public string? Model { get; set; }

    [JsonPropertyName("Motor_Hacmi")]
    public string? MotorHacmi { get; set; }

    [JsonPropertyName("Silindir_Adedi")]
    public string? SilindirAdedi { get; set; }

    [JsonPropertyName("Renk")]
    public string? Renk { get; set; }

    [JsonPropertyName("MlineID")]
    public string? MlineID { get; set; }

    [JsonPropertyName("TSE_No")]
    public string? TSENo { get; set; }

    [JsonPropertyName("Malin_Cinsi")]
    public string? MalinCinsi { get; set; }

    [JsonPropertyName("Motor_No")]
    public string? MotorNo { get; set; }

    [JsonPropertyName("IMEI")]
    public string? IMEI { get; set; }

    [JsonPropertyName("Order_No")]
    public string? OrderNo { get; set; }

    [JsonPropertyName("DtsDosyaNo")]
    public string? DtsDosyaNo { get; set; }

    [JsonPropertyName("FaturaNo")]
    public string? FaturaNo { get; set; }

    [JsonPropertyName("FaturaTarihi")]
    public string? FaturaTarihi { get; set; }

    [JsonPropertyName("TicariTanim")]
    public string? TicariTanim { get; set; }

    [JsonPropertyName("VitesTuru")]
    public string? VitesTuru { get; set; }

    [JsonPropertyName("MotorTipi")]
    public string? MotorTipi { get; set; }

    [JsonPropertyName("MotorGucu")]
    public decimal? MotorGucu { get; set; }

    [JsonPropertyName("Aktar1")]
    public string? Aktar1 { get; set; }

    [JsonPropertyName("Aktar2")]
    public string? Aktar2 { get; set; }

    [JsonPropertyName("Aktar3")]
    public string? Aktar3 { get; set; }

    [JsonPropertyName("Aktar4")]
    public string? Aktar4 { get; set; }

    [JsonPropertyName("Aktar5")]
    public string? Aktar5 { get; set; }

    [JsonPropertyName("Aktar6")]
    public string? Aktar6 { get; set; }

    [JsonPropertyName("Aktar7")]
    public string? Aktar7 { get; set; }

    [JsonPropertyName("Aktar8")]
    public string? Aktar8 { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  ODEME SEKLI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateOdemeSekli
{
    [JsonPropertyName("OdemeSekliKodu")]
    public int? OdemeSekliKodu { get; set; }

    [JsonPropertyName("OdemeTutari")]
    public decimal? OdemeTutari { get; set; }

    [JsonPropertyName("ImportLetterOfCreditDocumentCode")]
    public string? ImportLetterOfCreditDocumentCode { get; set; }

    [JsonPropertyName("TBFTarihi")]
    public string? TBFTarihi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  TCGB KAPATMA BILGI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateTcgbKapatma
{
    [JsonPropertyName("BWDeclarationNo")]
    public string? BWDeclarationNo { get; set; }

    [JsonPropertyName("Kapatilan_miktar")]
    public decimal? KapatilanMiktar { get; set; }

    [JsonPropertyName("LineNo")]
    public string? LineNo { get; set; }

    [JsonPropertyName("Aciklama")]
    public string? Aciklama { get; set; }

    [JsonPropertyName("BWDeclarationDate")]
    public string? BWDeclarationDate { get; set; }

    [JsonPropertyName("Kapatilan_kap_adedi")]
    public decimal? KapatilanKapAdedi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  DOKUMAN
// ════════════════════════════════════════════════════════════════
public class EvrimCreateDokuman
{
    [JsonPropertyName("Kod")]
    public string? Kod { get; set; }

    [JsonPropertyName("Tip")]
    public string? Tip { get; set; }

    [JsonPropertyName("Arti")]
    public string? Arti { get; set; }

    [JsonPropertyName("Dogrulama")]
    public string? Dogrulama { get; set; }

    [JsonPropertyName("Belge_tarihi")]
    public string? BelgeTarihi { get; set; }

    [JsonPropertyName("Referans")]
    public string? Referans { get; set; }

    [JsonPropertyName("Cikis_sira_no")]
    public int? CikisSiraNo { get; set; }

    [JsonPropertyName("Vize_tarihi")]
    public string? VizeTarihi { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  IS TAKIP
// ════════════════════════════════════════════════════════════════
public class EvrimCreateIsTakip
{
    [JsonPropertyName("Kod")]
    public string? Kod { get; set; }

    [JsonPropertyName("Aciklama")]
    public string? Aciklama { get; set; }

    [JsonPropertyName("TarihSaat")]
    public string? TarihSaat { get; set; }

    [JsonPropertyName("KulKod")]
    public string? KulKod { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  KONTEYNER BILGI
// ════════════════════════════════════════════════════════════════
public class EvrimCreateKonteyner
{
    [JsonPropertyName("Konteyner_No")]
    public string? KonteynerNo { get; set; }

    [JsonPropertyName("Ulke_Kodu")]
    public string? UlkeKodu { get; set; }

    [JsonPropertyName("KonteynerTipi")]
    public string? KonteynerTipi { get; set; }
}
