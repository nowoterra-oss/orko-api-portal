namespace Orko.Portal.Domain.Entities;

public class Archive
{
    public Guid Id { get; set; }
    public Guid DeclarationId { get; set; }
    public string FileName { get; set; } = default!;
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public string? DocumentType { get; set; }
    public string Base64Data { get; set; } = default!;
    public bool SentToEvrim { get; set; }
    public DateTime? SentAt { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Declaration Declaration { get; set; } = default!;
}
