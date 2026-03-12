using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Declarations;

public class SendToEvrimHandler
{
    private readonly PortalDbContext _db;
    private readonly IEvrimApiClient _evrim;
    private readonly ILogger<SendToEvrimHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public SendToEvrimHandler(PortalDbContext db, IEvrimApiClient evrim, ILogger<SendToEvrimHandler> logger)
    {
        _db = db;
        _evrim = evrim;
        _logger = logger;
    }

    public async Task<EvrimResponse> HandleAsync(Guid declarationId)
    {
        var declaration = await _db.Declarations
            .Include(d => d.WorkOrder)
            .FirstOrDefaultAsync(d => d.Id == declarationId);

        if (declaration == null)
            throw new KeyNotFoundException("Beyanname bulunamadi.");

        if (declaration.SentToEvrim)
            throw new InvalidOperationException("Beyanname zaten Evrim'e gonderilmis.");

        if (string.IsNullOrEmpty(declaration.DeclarationData))
            throw new InvalidOperationException("Beyanname verileri bos. Once formu doldurun.");

        // Format tespit: yeni mi eski mi?
        var format = DetectFormat(declaration.DeclarationData);

        EvrimResponse result;

        if (format == DataFormat.NewExport)
        {
            // Yeni export formati - olduğu gibi gonder
            var request = JsonSerializer.Deserialize<EvrimExportDeclarationRequest>(
                declaration.DeclarationData, JsonOptions) ?? new EvrimExportDeclarationRequest();
            result = await _evrim.CreateExportDeclarationAsync(request);
        }
        else if (format == DataFormat.NewImport)
        {
            // Yeni import formati - olduğu gibi gonder
            var request = JsonSerializer.Deserialize<EvrimCreateDeclarationRequest>(
                declaration.DeclarationData, JsonOptions) ?? new EvrimCreateDeclarationRequest();
            result = await _evrim.CreateImportDeclarationAsync(request);
        }
        else
        {
            // Eski format - mevcut davranis
            result = await SendOldFormat(declaration);
        }

        // Sonucu kaydet
        declaration.SentToEvrim = result.Success;
        declaration.SentAt = DateTime.UtcNow;
        declaration.EvrimResponse = result.RawResponse;
        declaration.EvrimDeclarationId = result.EvrimReferansNo;

        if (result.Success)
        {
            declaration.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.Status = WorkOrderStatus.EvrimeGonderildi;
            declaration.WorkOrder.UpdatedAt = DateTime.UtcNow;

            _db.StatusHistories.Add(new Domain.Entities.StatusHistory
            {
                Id = Guid.NewGuid(),
                WorkOrderId = declaration.WorkOrderId,
                FromStatus = WorkOrderStatus.Hazirlaniyor,
                ToStatus = WorkOrderStatus.EvrimeGonderildi,
                ChangedBy = "System",
                Note = $"Evrim'e basariyla gonderildi. EvrimRef: {result.EvrimReferansNo}",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Evrim gonderim: {FileNumber} | Format: {Format} | Basarili: {Success} | EvrimRef: {Ref} | Mesaj: {Msg}",
            declaration.WorkOrder.FileNumber, format, result.Success, result.EvrimReferansNo, result.ExceptionMessage);

        return result;
    }

    private async Task<EvrimResponse> SendOldFormat(Domain.Entities.Declaration declaration)
    {
        EvrimDeclarationRequest evrimRequest;
        try
        {
            evrimRequest = JsonSerializer.Deserialize<EvrimDeclarationRequest>(
                declaration.DeclarationData!, JsonOptions)
                ?? new EvrimDeclarationRequest();
        }
        catch (JsonException)
        {
            evrimRequest = new EvrimDeclarationRequest
            {
                ReferansNo = declaration.WorkOrder.FileNumber,
                Aciklamalar = declaration.DeclarationData
            };
        }

        evrimRequest.ReferansNo ??= declaration.WorkOrder.SelsilOrderId ?? declaration.WorkOrder.FileNumber;
        evrimRequest.DosyaNo ??= declaration.WorkOrder.FileNumber;

        if (declaration.DeclarationType == DeclarationType.Export)
            evrimRequest.Ihracat = true;
        else
            evrimRequest.Ihracat = null;

        CleanEmptyStrings(evrimRequest);
        if (evrimRequest.Kalemler != null)
        {
            foreach (var kalem in evrimRequest.Kalemler)
                CleanEmptyStrings(kalem);
        }

        var missingFields = new List<string>();
        if (string.IsNullOrEmpty(evrimRequest.DosyaNo)) missingFields.Add("dosyaNo (Beyanname No)");
        if (string.IsNullOrEmpty(evrimRequest.DosyaTarihi)) missingFields.Add("dosyaTarihi (Beyanname Tarihi)");
        if (string.IsNullOrEmpty(evrimRequest.Gumruk)) missingFields.Add("gumruk");
        if (string.IsNullOrEmpty(evrimRequest.MusteriVergi)) missingFields.Add("musteriVergi");
        if (string.IsNullOrEmpty(evrimRequest.MusteriUnvani)) missingFields.Add("musteriUnvani");
        if (evrimRequest.ToplamFatura is null or 0) missingFields.Add("toplamFatura");
        if (evrimRequest.Kalemler == null || evrimRequest.Kalemler.Count == 0) missingFields.Add("kalemler");

        if (missingFields.Count > 0)
            throw new InvalidOperationException(
                $"Beyanname verileri eksik. Lutfen formu doldurun. Eksik alanlar: {string.Join(", ", missingFields)}");

        if (declaration.DeclarationType == DeclarationType.Export)
            return await _evrim.CreateExportDeclarationAsync(evrimRequest);
        else
            return await _evrim.CreateImportDeclarationAsync(evrimRequest);
    }

    private enum DataFormat { OldFormat, NewExport, NewImport }

    private static DataFormat DetectFormat(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Yeni export: "Rejim" veya "Kalemler" (buyuk K) alanlari var
            if (root.TryGetProperty("Rejim", out _) || root.TryGetProperty("Kalemler", out _))
                return DataFormat.NewExport;

            // Yeni import: "RegimeCode" veya "Details" alanlari var
            if (root.TryGetProperty("RegimeCode", out _) || root.TryGetProperty("Details", out _))
                return DataFormat.NewImport;

            return DataFormat.OldFormat;
        }
        catch
        {
            return DataFormat.OldFormat;
        }
    }

    private static void CleanEmptyStrings(object obj)
    {
        foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite)
            {
                var val = (string?)prop.GetValue(obj);
                if (string.IsNullOrWhiteSpace(val))
                    prop.SetValue(obj, null);
            }
        }
    }
}
