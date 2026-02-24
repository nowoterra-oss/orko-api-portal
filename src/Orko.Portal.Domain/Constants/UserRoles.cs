namespace Orko.Portal.Domain.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Operator = "Operator";
    public const string Viewer = "Viewer";

    public static readonly string[] All = [Admin, Operator, Viewer];

    public static bool IsValid(string role) => All.Contains(role);
}
