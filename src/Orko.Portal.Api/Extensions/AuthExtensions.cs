using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Orko.Portal.Api.Extensions;

public static class AuthExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var sub = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    public static string GetUserRole(this HttpContext context)
    {
        return context.User.FindFirstValue("role")
               ?? context.User.FindFirstValue(ClaimTypes.Role)
               ?? "";
    }

    public static string GetUserEmail(this HttpContext context)
    {
        return context.User.FindFirstValue(JwtRegisteredClaimNames.Email)
               ?? context.User.FindFirstValue(ClaimTypes.Email)
               ?? "";
    }

    public static string GetUserName(this HttpContext context)
    {
        return context.User.FindFirstValue("name")
               ?? context.User.FindFirstValue(ClaimTypes.Name)
               ?? "";
    }

    public static bool IsInRole(this HttpContext context, params string[] roles)
    {
        var userRole = context.GetUserRole();
        return roles.Contains(userRole);
    }
}
