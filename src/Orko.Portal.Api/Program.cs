using Hangfire;
using Microsoft.EntityFrameworkCore;
using Orko.Portal.Api.Endpoints;
using Orko.Portal.Api.Middleware;
using Orko.Portal.Application.Archives;
using Orko.Portal.Application.Dashboard;
using Orko.Portal.Application.Declarations;
using Orko.Portal.Application.Statuses;
using Orko.Portal.Application.WorkOrders;
using Orko.Portal.Domain.Interfaces;
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

// --- Application Services ---
builder.Services.AddScoped<CreateWorkOrderHandler>();
builder.Services.AddScoped<GetWorkOrdersHandler>();
builder.Services.AddScoped<GetDeclarationHandler>();
builder.Services.AddScoped<UpdateDeclarationHandler>();
builder.Services.AddScoped<SendToEvrimHandler>();
builder.Services.AddScoped<UploadAndSendHandler>();
builder.Services.AddScoped<UpdateStatusHandler>();
builder.Services.AddScoped<UploadArchiveHandler>();
builder.Services.AddScoped<DashboardHandler>();

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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
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

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// --- Endpoints ---
app.MapWorkOrderEndpoints();
app.MapDeclarationEndpoints();
app.MapStatusEndpoints();
app.MapArchiveEndpoints();
app.MapDashboardEndpoints();
app.MapLogEndpoints();

app.MapHangfireDashboard("/hangfire");

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

app.Run();
