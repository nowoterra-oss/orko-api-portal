namespace Orko.Portal.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "Operator";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
