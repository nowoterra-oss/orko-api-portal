namespace Orko.Portal.Contracts.Declarations;

public class UploadAndSendDto
{
    public string FileContent { get; set; } = "";
    public string FileFormat { get; set; } = "json"; // "json" veya "xml"
}
