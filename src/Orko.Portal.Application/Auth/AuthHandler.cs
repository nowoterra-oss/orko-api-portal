using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Auth;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Auth;

public class AuthHandler
{
    private readonly PortalDbContext _db;
    private readonly JwtTokenService _tokenService;

    public AuthHandler(PortalDbContext db, JwtTokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var token = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            }
        };
    }

    public async Task<SetupStatusDto> GetSetupStatusAsync()
    {
        var userCount = await _db.Users.CountAsync();
        return new SetupStatusDto
        {
            IsSetupComplete = userCount > 0,
            UserCount = userCount
        };
    }

    public async Task<LoginResponseDto?> SetupAsync(SetupDto dto)
    {
        // Only allow setup if no users exist
        var hasUsers = await _db.Users.AnyAsync();
        if (hasUsers)
            return null;

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRoles.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            }
        };
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
        if (user == null)
            return null;

        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
