namespace Orko.Portal.Contracts.Users;

public class UpdateUserDto
{
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public string? Password { get; set; }
}
