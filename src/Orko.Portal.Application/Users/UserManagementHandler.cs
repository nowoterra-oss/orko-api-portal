using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Users;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Users;

public class UserManagementHandler
{
    private readonly PortalDbContext _db;

    public UserManagementHandler(PortalDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserListDto>> GetAllAsync()
    {
        return await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserListDto> CreateAsync(CreateUserDto dto)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            throw new InvalidOperationException("Bu e-posta adresi zaten kullanılıyor.");

        if (!UserRoles.IsValid(dto.Role))
            throw new ArgumentException($"Geçersiz rol: {dto.Role}");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserListDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserListDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return null;

        if (dto.FullName != null)
            user.FullName = dto.FullName;

        if (dto.Role != null)
        {
            if (!UserRoles.IsValid(dto.Role))
                throw new ArgumentException($"Geçersiz rol: {dto.Role}");
            user.Role = dto.Role;
        }

        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _db.SaveChangesAsync();

        return new UserListDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return false;

        // Soft delete
        user.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }
}
