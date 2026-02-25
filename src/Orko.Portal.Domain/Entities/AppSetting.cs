namespace Orko.Portal.Domain.Entities;

public class AppSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
