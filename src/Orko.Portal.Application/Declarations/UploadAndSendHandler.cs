using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Orko.Portal.Contracts.Declarations;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class UploadAndSendHandler
{
    private readonly PortalDbContext _db;
    private readonly SendToEvrimHandler _sendHandler;
    private readonly ILogger<UploadAndSendHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UploadAndSendHandler(
        PortalDbContext db,
        SendToEvrimHandler sendHandler,
        ILogger<UploadAndSendHandler> logger)
    {
        _db = db;
        _sendHandler = sendHandler;
        _logger = logger;
    }

    public async Task<EvrimResponse> HandleAsync(Guid declarationId, UploadAndSendDto dto)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        if (declaration.SentToEvrim)
            throw new InvalidOperationException("Beyanname zaten Evrim'e gonderilmis.");

        if (string.IsNullOrWhiteSpace(dto.FileContent))
            throw new InvalidOperationException("Dosya icerigi bos.");

        // Dosya icerigini parse et ve JSON olarak DeclarationData'ya yaz
        EvrimDeclarationRequest evrimRequest;
        var format = dto.FileFormat?.ToLowerInvariant() ?? "json";

        if (format == "xml")
        {
            evrimRequest = DeserializeXml(dto.FileContent);
        }
        else
        {
            evrimRequest = JsonSerializer.Deserialize<EvrimDeclarationRequest>(
                dto.FileContent, JsonOptions)
                ?? throw new InvalidOperationException("JSON dosyasi parse edilemedi.");
        }

        // Parse edilen veriyi JSON olarak DeclarationData'ya kaydet
        declaration.DeclarationData = JsonSerializer.Serialize(evrimRequest, JsonOptions);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Dosyadan yuklendi: {FileNumber} | Format: {Format}",
            declaration.WorkOrder.FileNumber, format);

        // Mevcut send handler'i cagir (validasyon + Evrim gonderimi)
        return await _sendHandler.HandleAsync(declarationId);
    }

    private static EvrimDeclarationRequest DeserializeXml(string xmlContent)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(EvrimDeclarationRequest),
                new XmlRootAttribute("declaration"));
            using var reader = new StringReader(xmlContent);
            return (EvrimDeclarationRequest)(serializer.Deserialize(reader)
                ?? throw new InvalidOperationException("XML dosyasi parse edilemedi."));
        }
        catch (InvalidOperationException ex) when (ex.InnerException != null)
        {
            throw new InvalidOperationException(
                $"XML parse hatasi: {ex.InnerException.Message}");
        }
    }
}
