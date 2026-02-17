using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Contracts.Declarations;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class UpdateDeclarationHandler
{
    private readonly PortalDbContext _db;
    private readonly ILogger<UpdateDeclarationHandler> _logger;

    public UpdateDeclarationHandler(PortalDbContext db, ILogger<UpdateDeclarationHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(Guid declarationId, UpdateDeclarationDto dto)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null) return false;

        if (declaration.SentToEvrim)
            throw new InvalidOperationException("Evrim'e gonderilmis beyanname duzenlenemez.");

        declaration.DeclarationData = dto.DeclarationData;
        declaration.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Beyanname guncellendi: {FileNumber}", declaration.WorkOrder.FileNumber);
        return true;
    }
}
