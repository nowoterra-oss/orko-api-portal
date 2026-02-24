using System.Diagnostics;
using Orko.Portal.Api.Extensions;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, PortalDbContext db)
    {
        // Sadece /api path'lerini logla
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();

        // Request body oku
        context.Request.EnableBuffering();
        string requestBody;
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // Response body yakala
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        sw.Stop();

        // Response body oku
        responseBodyStream.Position = 0;
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Position = 0;
        await responseBodyStream.CopyToAsync(originalBodyStream);

        // DB'ye logla
        try
        {
            var log = new ApiLog
            {
                Direction = "INCOMING",
                Endpoint = context.Request.Path,
                Method = context.Request.Method,
                RequestBody = requestBody.Length > 10000 ? requestBody[..10000] + "...[truncated]" : requestBody,
                ResponseBody = responseBody.Length > 10000 ? responseBody[..10000] + "...[truncated]" : responseBody,
                StatusCode = context.Response.StatusCode,
                DurationMs = (int)sw.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.GetUserId();
                log.UserId = userId != Guid.Empty ? userId : null;
                log.UserName = context.GetUserName();
                log.UserEmail = context.GetUserEmail();
                log.UserRole = context.GetUserRole();
            }

            db.ApiLogs.Add(log);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API log kaydedilemedi.");
        }
    }
}
