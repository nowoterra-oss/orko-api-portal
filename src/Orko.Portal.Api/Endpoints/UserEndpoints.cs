using Orko.Portal.Api.Extensions;
using Orko.Portal.Application.Users;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.Users;
using Orko.Portal.Domain.Constants;

namespace Orko.Portal.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Kullanici Yonetimi");

        // GET /api/users
        group.MapGet("/", async (HttpContext context, UserManagementHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var result = await handler.GetAllAsync();
            return Results.Ok(ApiResponse<List<UserListDto>>.Ok(result));
        })
        .WithName("GetUsers");

        // POST /api/users
        group.MapPost("/", async (HttpContext context, CreateUserDto dto, UserManagementHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            try
            {
                var result = await handler.CreateAsync(dto);
                return Results.Created($"/api/users/{result.Id}", ApiResponse<UserListDto>.Ok(result, "Kullanıcı oluşturuldu."));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateUser");

        // PUT /api/users/{id}
        group.MapPut("/{id:guid}", async (Guid id, HttpContext context, UpdateUserDto dto, UserManagementHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            try
            {
                var result = await handler.UpdateAsync(id, dto);
                if (result == null)
                    return Results.NotFound(ApiResponse<object>.Fail("Kullanıcı bulunamadı."));

                return Results.Ok(ApiResponse<UserListDto>.Ok(result, "Kullanıcı güncellendi."));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("UpdateUser");

        // DELETE /api/users/{id}
        group.MapDelete("/{id:guid}", async (Guid id, HttpContext context, UserManagementHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var result = await handler.DeleteAsync(id);
            if (!result)
                return Results.NotFound(ApiResponse<object>.Fail("Kullanıcı bulunamadı."));

            return Results.Ok(ApiResponse<object>.Ok(new { }, "Kullanıcı deaktif edildi."));
        })
        .WithName("DeleteUser");
    }
}
