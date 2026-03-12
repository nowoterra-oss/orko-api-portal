using Hangfire;
using Microsoft.EntityFrameworkCore;
using Orko.Portal.Api.Endpoints;
using Orko.Portal.Api.Middleware;
using Orko.Portal.Application.Archives;
using Orko.Portal.Application.Auth;
using Orko.Portal.Application.Dashboard;
using Orko.Portal.Application.Declarations;
using Orko.Portal.Application.Statuses;
using Orko.Portal.Application.Users;
using Orko.Portal.Application.WorkOrders;
using Orko.Portal.Application.Settings;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.Email;
using Orko.Portal.Infrastructure.ExternalServices;
using Orko.Portal.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Database ---
builder.Services.AddDbContext<PortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// --- Hangfire ---
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();

// --- Evrim HTTP Client ---
builder.Services.AddHttpClient<IEvrimApiClient, EvrimApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Evrim:BaseUrl"] ?? "https://api.evrim.com/EvrimEntRestWS.dll/");
    client.Timeout = TimeSpan.FromSeconds(120);
});

// --- HttpContextAccessor (OUTGOING loglarda kullanıcı bilgisi için) ---
builder.Services.AddHttpContextAccessor();

// --- Auth Services ---
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<UserManagementHandler>();

// --- Email Service ---
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// --- Application Services ---
builder.Services.AddScoped<SettingsHandler>();
builder.Services.AddScoped<CreateWorkOrderHandler>();
builder.Services.AddScoped<GetWorkOrdersHandler>();
builder.Services.AddScoped<GetDeclarationHandler>();
builder.Services.AddScoped<UpdateDeclarationHandler>();
builder.Services.AddScoped<SendToEvrimHandler>();
builder.Services.AddScoped<UploadAndSendHandler>();
builder.Services.AddScoped<UpdateStatusHandler>();
builder.Services.AddScoped<UploadArchiveHandler>();
builder.Services.AddScoped<DashboardHandler>();
builder.Services.AddScoped<CreateDirectDeclarationHandler>();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Orko Portal API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API Key (X-API-Key header)"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- CORS (Next.js frontend icin) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration["Frontend:Url"] ?? "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// --- Middleware Pipeline ---
app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orko Portal API v1"));

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseMiddleware<JwtAuthMiddleware>();

// --- Endpoints ---
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapWorkOrderEndpoints();
app.MapDeclarationEndpoints();
app.MapStatusEndpoints();
app.MapArchiveEndpoints();
app.MapDashboardEndpoints();
app.MapLogEndpoints();
app.MapSettingsEndpoints();
app.MapDirectDeclarationEndpoints();

app.MapHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
{
    Authorization = [new Orko.Portal.Api.Filters.HangfireAuthFilter()]
});

// --- Health Check ---
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithTags("System");

// --- Auto Migration (development only) ---
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
    await db.Database.MigrateAsync();
}

// --- Seed AppSettings & Register Recurring Jobs ---
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();

    try
    {
        var hasSettings = await db.AppSettings.AnyAsync();
        if (!hasSettings)
        {
            var now = DateTime.UtcNow;
            var defaults = new[]
            {
                // SMTP
                new { Key = SettingKeys.SmtpHost, Value = "", Desc = "SMTP sunucu adresi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpPort, Value = "587", Desc = "SMTP port numarasi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpUsername, Value = "", Desc = "SMTP kullanici adi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpPassword, Value = "", Desc = "SMTP sifresi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpFromAddress, Value = "", Desc = "Gonderen e-posta adresi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpFromName, Value = "Orko Portal", Desc = "Gonderen adi", Cat = SettingKeys.CategorySmtp },
                new { Key = SettingKeys.SmtpEnableSsl, Value = "true", Desc = "SSL kullan", Cat = SettingKeys.CategorySmtp },
                // WorkOrderEmail
                new { Key = SettingKeys.WorkOrderEmailEnabled, Value = "false", Desc = "Is emri e-posta bildirimi aktif", Cat = SettingKeys.CategoryWorkOrderEmail },
                new { Key = SettingKeys.WorkOrderEmailRecipients, Value = "", Desc = "Alicilar (virgul ile ayrilmis)", Cat = SettingKeys.CategoryWorkOrderEmail },
                new { Key = SettingKeys.WorkOrderEmailSubject, Value = "Yeni Is Emri: {FileNumber}", Desc = "E-posta konusu", Cat = SettingKeys.CategoryWorkOrderEmail },
                new { Key = SettingKeys.WorkOrderEmailTemplate, Value = "", Desc = "E-posta HTML sablonu", Cat = SettingKeys.CategoryWorkOrderEmail },
                // Reminder
                new { Key = SettingKeys.ReminderEnabled, Value = "false", Desc = "Hatirlatma servisi aktif", Cat = SettingKeys.CategoryReminder },
                new { Key = SettingKeys.ReminderCronExpression, Value = "0 9 * * 1-5", Desc = "Cron ifadesi (varsayilan: hafta ici 09:00)", Cat = SettingKeys.CategoryReminder },
                new { Key = SettingKeys.ReminderRecipients, Value = "", Desc = "Alicilar (virgul ile ayrilmis)", Cat = SettingKeys.CategoryReminder },
                new { Key = SettingKeys.ReminderDraftThresholdHours, Value = "24", Desc = "Taslak esik suresi (saat)", Cat = SettingKeys.CategoryReminder },
                new { Key = SettingKeys.ReminderSubject, Value = "Taslak Is Emirleri Hatirlatmasi ({Count} adet)", Desc = "E-posta konusu", Cat = SettingKeys.CategoryReminder },
                new { Key = SettingKeys.ReminderTemplate, Value = "", Desc = "E-posta HTML sablonu", Cat = SettingKeys.CategoryReminder },
            };

            foreach (var d in defaults)
            {
                db.AppSettings.Add(new AppSetting
                {
                    Key = d.Key,
                    Value = d.Value,
                    Description = d.Desc,
                    Category = d.Cat,
                    UpdatedAt = now,
                    UpdatedBy = "System"
                });
            }
            await db.SaveChangesAsync();
            Log.Information("AppSettings varsayilan degerler olusturuldu.");
        }

        // Reminder recurring job: enabled ise kaydet
        var reminderEnabled = await db.AppSettings
            .FirstOrDefaultAsync(s => s.Key == SettingKeys.ReminderEnabled);
        var reminderCron = await db.AppSettings
            .FirstOrDefaultAsync(s => s.Key == SettingKeys.ReminderCronExpression);

        if (reminderEnabled?.Value?.Equals("true", StringComparison.OrdinalIgnoreCase) == true
            && !string.IsNullOrEmpty(reminderCron?.Value))
        {
            var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobs.AddOrUpdate<DraftReminderJob>(
                "draft-reminder",
                job => job.ExecuteAsync(),
                reminderCron.Value);
            Log.Information("Hatirlatma recurring job kaydedildi: {Cron}", reminderCron.Value);
        }
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "AppSettings seed sirasinda hata (tablo henuz olusturulmamis olabilir)");
    }
}

app.Run();
