namespace Orko.Portal.Contracts.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public UserInfoDto User { get; set; } = default!;
}
