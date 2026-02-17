namespace Orko.Portal.Contracts.Archives;

public class UploadArchiveDto
{
    public string FileName { get; set; } = default!;
    public string Base64Data { get; set; } = default!;
    public string? ContentType { get; set; }
    public string? DocumentType { get; set; }
}
