namespace Orko.Portal.Contracts.Settings;

public class AppSettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class SettingItemDto
{
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}

public class UpdateSettingsDto
{
    public List<SettingItemDto> Settings { get; set; } = new();
}

public class SmtpTestResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
