namespace Orko.Portal.Contracts.Auth;

public class SetupDto
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
