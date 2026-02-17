using System.Text.Json.Serialization;

namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

/// <summary>
/// Evrim "referans" sema - POST /api/sendworkorder, /api/sendworkorderarchive
/// </summary>
public class EvrimReferansRequest
{
    [JsonPropertyName("company")]
    public int? Company { get; set; }

    [JsonPropertyName("siraNo")]
    public int? SiraNo { get; set; }

    [JsonPropertyName("refFile")]
    public string? RefFile { get; set; }

    [JsonPropertyName("userId")]
    public int? UserId { get; set; }
}
