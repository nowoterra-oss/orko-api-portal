namespace Orko.Portal.Contracts.Users;

public class CreateUserDto
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
}
