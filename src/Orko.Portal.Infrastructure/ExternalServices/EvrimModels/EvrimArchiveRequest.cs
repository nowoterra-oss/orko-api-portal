using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim "Arsiv" sema
/// </summary>
public class EvrimArchiveRequest
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = default!;  // max 100

    [JsonPropertyName("fileRefNo")]
    public string? FileRefNo { get; set; }  // Dosya referans (max 100)

    [JsonPropertyName("fileData")]
    public string FileData { get; set; } = default!;  // Base64

    [JsonPropertyName("belgeKod")]
    public string? BelgeKod { get; set; }  // max 4

    [JsonPropertyName("tarih")]
    public string? Tarih { get; set; }  // yyyy-MM-ddTHH:mm:ss

    [JsonPropertyName("kullaniciKod")]
    public string? KullaniciKod { get; set; }  // max 10

    [JsonPropertyName("aciklama")]
    public string? Aciklama { get; set; }  // max 100

    [JsonPropertyName("grupKodu")]
    public string? GrupKodu { get; set; }  // max 20

    [JsonPropertyName("referans")]
    public string? Referans { get; set; }  // max 70

    [JsonPropertyName("eklemeTipi")]
    public string? EklemeTipi { get; set; }  // G: Gumruk, F: Firma
}
