using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.WorkOrders;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.WorkOrders;

public class GetWorkOrdersHandler
{
    private readonly PortalDbContext _db;

    public GetWorkOrdersHandler(PortalDbContext db) => _db = db;

    public async Task<PagedResult<WorkOrderListDto>> HandleAsync(
        int page = 1, int pageSize = 20, string? status = null, string? type = null, string? search = null)
    {
        var query = _db.WorkOrders
            .Include(w => w.Declaration)
            .AsQueryable();

        // Filtreler
        if (!string.IsNullOrEmpty(status))
            query = query.Where(w => w.Status.ToString() == status);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(w => w.Type.ToString() == type);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(w =>
                w.FileNumber.Contains(search) ||
                (w.SelsilOrderId != null && w.SelsilOrderId.Contains(search)));

        var totalCount = await query.CountAsync();

        var rawItems = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(w => new
            {
                w.Id,
                w.FileNumber,
                w.SelsilOrderId,
                Type = w.Type.ToString(),
                Status = w.Status.ToString(),
                HasDeclaration = w.Declaration != null,
                SentToEvrim = w.Declaration != null && w.Declaration.SentToEvrim,
                w.CreatedAt,
                w.SelsilPayload
            })
            .ToListAsync();

        var items = rawItems.Select(w => new WorkOrderListDto
        {
            Id = w.Id,
            FileNumber = w.FileNumber,
            SelsilOrderId = w.SelsilOrderId,
            Type = w.Type,
            Status = w.Status,
            HasDeclaration = w.HasDeclaration,
            SentToEvrim = w.SentToEvrim,
            CreatedAt = w.CreatedAt,
            CustomerName = ExtractCustomerName(w.SelsilPayload)
        }).ToList();

        return new PagedResult<WorkOrderListDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<WorkOrderResponseDto?> GetByFileNumberAsync(string fileNumber)
    {
        var w = await _db.WorkOrders
            .Include(w => w.Declaration)
            .FirstOrDefaultAsync(w => w.FileNumber == fileNumber);

        if (w == null) return null;

        return new WorkOrderResponseDto
        {
            Id = w.Id,
            FileNumber = w.FileNumber,
            SelsilOrderId = w.SelsilOrderId,
            Type = w.Type.ToString(),
            Status = w.Status.ToString(),
            CreatedBy = w.CreatedBy,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt,
            Declaration = w.Declaration == null ? null : new Contracts.Declarations.DeclarationSummaryDto
            {
                Id = w.Declaration.Id,
                DeclarationType = w.Declaration.DeclarationType.ToString(),
                Status = w.Declaration.Status.ToString(),
                EvrimDeclarationId = w.Declaration.EvrimDeclarationId,
                SentToEvrim = w.Declaration.SentToEvrim,
                SentAt = w.Declaration.SentAt
            }
        };
    }

    private static string? ExtractCustomerName(string? payload)
    {
        if (string.IsNullOrEmpty(payload)) return null;
        try
        {
            using var doc = JsonDocument.Parse(payload);
            if (doc.RootElement.TryGetProperty("CustomerName", out var prop))
                return prop.GetString();
            if (doc.RootElement.TryGetProperty("customerName", out prop))
                return prop.GetString();
            return null;
        }
        catch { return null; }
    }
}
