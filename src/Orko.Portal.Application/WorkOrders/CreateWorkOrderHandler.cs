using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Contracts.WorkOrders;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.WorkOrders;

public class CreateWorkOrderHandler
{
    private readonly PortalDbContext _db;
    private readonly ILogger<CreateWorkOrderHandler> _logger;

    public CreateWorkOrderHandler(PortalDbContext db, ILogger<CreateWorkOrderHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<WorkOrderResponseDto> HandleAsync(CreateWorkOrderDto dto)
    {
        // Tip parse
        if (!Enum.TryParse<DeclarationType>(dto.Type, true, out var declarationType))
            throw new ArgumentException($"Gecersiz tip: {dto.Type}. 'Import' veya 'Export' olmali.");

        // Dosya numarasi uret
        var today = DateTime.UtcNow;
        var count = await _db.WorkOrders
            .CountAsync(w => w.CreatedAt.Date == today.Date);
        var fileNumber = $"ORK-{today:yyyyMMdd}-{(count + 1):D4}";

        // Is emri olustur
        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            FileNumber = fileNumber,
            SelsilOrderId = dto.SelsilOrderId,
            Type = declarationType,
            Status = WorkOrderStatus.Taslak,
            SelsilPayload = System.Text.Json.JsonSerializer.Serialize(dto),
            CreatedAt = today,
            UpdatedAt = today
        };

        // Taslak beyanname olustur
        var declaration = new Declaration
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            DeclarationType = declarationType,
            Status = WorkOrderStatus.Taslak,
            CreatedAt = today,
            UpdatedAt = today
        };

        _db.WorkOrders.Add(workOrder);
        _db.Declarations.Add(declaration);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Is emri olusturuldu: {FileNumber}, Tip: {Type}", fileNumber, dto.Type);

        return new WorkOrderResponseDto
        {
            Id = workOrder.Id,
            FileNumber = fileNumber,
            SelsilOrderId = dto.SelsilOrderId,
            Type = dto.Type,
            Status = WorkOrderStatus.Taslak.ToString(),
            CreatedAt = workOrder.CreatedAt,
            UpdatedAt = workOrder.UpdatedAt,
            Declaration = new Contracts.Declarations.DeclarationSummaryDto
            {
                Id = declaration.Id,
                DeclarationType = declaration.DeclarationType.ToString(),
                Status = declaration.Status.ToString(),
                SentToEvrim = false
            }
        };
    }
}
