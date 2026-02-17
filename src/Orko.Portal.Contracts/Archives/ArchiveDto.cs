namespace Orko.Portal.Contracts.Archives;

public class ArchiveDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public string? DocumentType { get; set; }
    public bool SentToEvrim { get; set; }
    public DateTime? SentAt { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
