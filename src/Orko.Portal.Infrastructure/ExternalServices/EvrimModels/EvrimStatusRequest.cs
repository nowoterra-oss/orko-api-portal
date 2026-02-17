using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim "Status" sema - POST /api/status icin
/// </summary>
public class EvrimStatusRequest
{
    [JsonPropertyName("evrakTip")]
    public int EvrakTip { get; set; }  // 0: Beyanname, 1: Is Emri

    [JsonPropertyName("musavirVergiNo")]
    public string? MusavirVergiNo { get; set; }

    [JsonPropertyName("beyannameId")]
    public int? BeyannameId { get; set; }

    [JsonPropertyName("tip")]
    public string? Tip { get; set; }  // T: Ithalat, H: Ihracat

    [JsonPropertyName("dosyaNo")]
    public string? DosyaNo { get; set; }

    [JsonPropertyName("referansNo")]
    public string? ReferansNo { get; set; }

    [JsonPropertyName("isTakipler")]
    public List<EvrimIsTakip>? IsTakipler { get; set; }
}

public class EvrimIsTakip
{
    [JsonPropertyName("kod")]
    public string? Kod { get; set; }  // Is takip kodu (max 3)

    [JsonPropertyName("tarihSaat")]
    public string? TarihSaat { get; set; }  // yyyy-MM-ddTHH:mm:ss

    [JsonPropertyName("aciklama")]
    public string? Aciklama { get; set; }  // max 100

    [JsonPropertyName("kullaniciKod")]
    public string? KullaniciKod { get; set; }  // max 50

    [JsonPropertyName("isNot")]
    public string? IsNot { get; set; }  // max 100
}
