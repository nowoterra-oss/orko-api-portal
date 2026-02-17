namespace Orko.Portal.Domain.Entities;

public class ApiLog
{
    public long Id { get; set; }
    public string Direction { get; set; } = default!;
    public string Endpoint { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int StatusCode { get; set; }
    public int DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
