namespace Orko.Portal.Contracts.Declarations;

public class UploadFileDto
{
    public string FileName { get; set; } = "";
    public string FileContent { get; set; } = "";
}

public class UploadAndSendDto
{
    public string FileContent { get; set; } = "";
    public string FileFormat { get; set; } = "json"; // "json" veya "xml"
    public List<UploadFileDto>? AdditionalFiles { get; set; }
}
