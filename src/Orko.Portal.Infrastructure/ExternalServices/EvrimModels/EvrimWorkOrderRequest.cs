using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim "WorkOrder" sema - POST /api/workorder
/// </summary>
public class EvrimWorkOrderRequest
{
    [JsonPropertyName("refNo")]
    public string? RefNo { get; set; }  // max 50

    [JsonPropertyName("ticaretTipi")]
    public int TicaretTipi { get; set; }  // 1: Ithalat, 2: Ihracat, 3: ANT

    [JsonPropertyName("teslimSekli")]
    public string? TeslimSekli { get; set; }  // CFR, CIF vs (max 3)

    [JsonPropertyName("yuklemeNoktasi")]
    public string? YuklemeNoktasi { get; set; }  // Gumruk kodu (max 15)

    [JsonPropertyName("varisNoktasi")]
    public string? VarisNoktasi { get; set; }

    [JsonPropertyName("tasimaSekli")]
    public string? TasimaSekli { get; set; }  // max 2

    [JsonPropertyName("gumruk")]
    public string? Gumruk { get; set; }  // max 15

    [JsonPropertyName("kapAdedi")]
    public int? KapAdedi { get; set; }

    [JsonPropertyName("yurtDisiMusteri")]
    public string? YurtDisiMusteri { get; set; }  // VKN max 11

    [JsonPropertyName("yurtIciMusteri")]
    public string? YurtIciMusteri { get; set; }  // VKN max 11

    [JsonPropertyName("toplamFaturaBedeli")]
    public decimal? ToplamFaturaBedeli { get; set; }

    [JsonPropertyName("dovizCinsi")]
    public string? DovizCinsi { get; set; }  // max 3

    [JsonPropertyName("toplamBrutKg")]
    public decimal? ToplamBrutKg { get; set; }

    [JsonPropertyName("toplamNetKg")]
    public decimal? ToplamNetKg { get; set; }

    [JsonPropertyName("rejim")]
    public string? Rejim { get; set; }  // max 100

    [JsonPropertyName("emirNot")]
    public string? EmirNot { get; set; }  // max 100

    [JsonPropertyName("yuklemeTarihi")]
    public string? YuklemeTarihi { get; set; }  // yyyy-MM-ddTHH:mm:ss

    [JsonPropertyName("tahminiVarisTarihi")]
    public string? TahminiVarisTarihi { get; set; }

    [JsonPropertyName("kalemler")]
    public List<EvrimWorkOrderKalem>? Kalemler { get; set; }
}

public class EvrimWorkOrderKalem
{
    [JsonPropertyName("siraNo")]
    public int SiraNo { get; set; }

    [JsonPropertyName("urunKodu")]
    public string? UrunKodu { get; set; }  // max 100

    [JsonPropertyName("faturaNumarasi")]
    public string? FaturaNumarasi { get; set; }  // max 100

    [JsonPropertyName("faturaTarihi")]
    public string? FaturaTarihi { get; set; }

    [JsonPropertyName("urunAdi")]
    public string? UrunAdi { get; set; }  // max 500

    [JsonPropertyName("gtip")]
    public string? Gtip { get; set; }  // max 15

    [JsonPropertyName("mensei")]
    public string? Mensei { get; set; }  // TR, DE, FR (max 20)

    [JsonPropertyName("miktar")]
    public decimal? Miktar { get; set; }

    [JsonPropertyName("miktarCinsi")]
    public string? MiktarCinsi { get; set; }  // C62, KGM, PR (max 3)

    [JsonPropertyName("brutAgirlik")]
    public decimal? BrutAgirlik { get; set; }

    [JsonPropertyName("netAgirlik")]
    public decimal? NetAgirlik { get; set; }

    [JsonPropertyName("tutar")]
    public decimal? Tutar { get; set; }

    [JsonPropertyName("dovizCinsi")]
    public string? DovizCinsi { get; set; }  // max 15
}
