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

        // JSON'dan Evrim Declaration modeline donustur
        EvrimDeclarationRequest evrimRequest;
        try
        {
            evrimRequest = JsonSerializer.Deserialize<EvrimDeclarationRequest>(
                declaration.DeclarationData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new EvrimDeclarationRequest();
        }
        catch (JsonException)
        {
            // Eski format ise wrapper olarak gonder
            evrimRequest = new EvrimDeclarationRequest
            {
                ReferansNo = declaration.WorkOrder.FileNumber,
                Aciklamalar = declaration.DeclarationData
            };
        }

        // Referans numarasini her zaman set et
        evrimRequest.ReferansNo ??= declaration.WorkOrder.FileNumber;
        evrimRequest.DosyaNo ??= declaration.WorkOrder.FileNumber;

        // ihracat sadece export ise true olarak gonder, import ise null birak (json'dan exclude edilir)
        if (declaration.DeclarationType == DeclarationType.Export)
            evrimRequest.Ihracat = true;
        else
            evrimRequest.Ihracat = null;

        // Bos stringleri null yap (WhenWritingNull ile json'dan exclude edilsinler)
        CleanEmptyStrings(evrimRequest);
        if (evrimRequest.Kalemler != null)
        {
            foreach (var kalem in evrimRequest.Kalemler)
                CleanEmptyStrings(kalem);
        }

        // Zorunlu alan kontrolu
        var missingFields = new List<string>();
        if (string.IsNullOrEmpty(evrimRequest.DosyaNo)) missingFields.Add("dosyaNo");
        if (string.IsNullOrEmpty(evrimRequest.Gumruk)) missingFields.Add("gumruk");
        if (string.IsNullOrEmpty(evrimRequest.MusteriVergi)) missingFields.Add("musteriVergi");
        if (string.IsNullOrEmpty(evrimRequest.MusteriUnvani)) missingFields.Add("musteriUnvani");
        if (evrimRequest.ToplamFatura is null or 0) missingFields.Add("toplamFatura");
        if (evrimRequest.Kalemler == null || evrimRequest.Kalemler.Count == 0) missingFields.Add("kalemler");

        if (missingFields.Count > 0)
            throw new InvalidOperationException(
                $"Beyanname verileri eksik. Lutfen formu doldurun. Eksik alanlar: {string.Join(", ", missingFields)}");

        // Evrim'e gonder (tip'e gore import/export)
        EvrimResponse result;
        if (declaration.DeclarationType == DeclarationType.Export)
        {
            result = await _evrim.CreateExportDeclarationAsync(evrimRequest);
        }
        else
        {
            result = await _evrim.CreateImportDeclarationAsync(evrimRequest);
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
            "Evrim gonderim: {FileNumber} | Basarili: {Success} | EvrimRef: {Ref} | Mesaj: {Msg}",
            declaration.WorkOrder.FileNumber, result.Success, result.EvrimReferansNo, result.ExceptionMessage);

        return result;
    }

    /// <summary>
    /// Bos string property'leri null yapar, WhenWritingNull ile json'dan exclude edilir
    /// </summary>
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
