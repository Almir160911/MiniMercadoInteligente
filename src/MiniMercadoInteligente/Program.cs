using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Application.Background;
using MiniMercadoInteligente.Application.Services;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;
using MiniMercadoInteligente.Infrastructure.Repositories;
using MiniMercadoInteligente.Infrastructure.Vision;
using MiniMercadoInteligente.Infrastructure.Devices;

var builder = WebApplication.CreateBuilder(args);

// =========================
// CONFIG
// =========================
var configuration = builder.Configuration;

// =========================
// DATABASE
// =========================
var connectionString =
    configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=minimercado;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// =========================
// SERVICES
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================
// REPOSITORIES
// =========================
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
builder.Services.AddScoped<IEventStore, EventStoreRepository>();

builder.Services.AddScoped<IFraudEngineService, FraudEngineService>();

builder.Services.AddScoped<IFraudRule, QuantityDivergenceFraudRule>();
builder.Services.AddScoped<IFraudRule, ExitWithoutPaymentFraudRule>();
builder.Services.AddScoped<IFraudRule, WeightEventWithoutCartFraudRule>();
builder.Services.AddScoped<IFraudRule, BurstEventFraudRule>();

builder.Services.AddScoped<IAreaRepository, AreaRepository>();

builder.Services.AddScoped<IDeviceApiKeyRepository, DeviceApiKeyRepository>();

builder.Services.AddScoped<IFraudEngineService, FraudEngineService>();

builder.Services.AddScoped<IFraudRule, QuantityDivergenceFraudRule>();
builder.Services.AddScoped<IFraudRule, ExitWithoutPaymentFraudRule>();
builder.Services.AddScoped<IFraudRule, WeightEventWithoutCartFraudRule>();
builder.Services.AddScoped<IFraudRule, BurstEventFraudRule>();
builder.Services.AddScoped<IFraudRule, TrackingWithoutSessionFraudRule>();
builder.Services.AddScoped<IFraudRule, WeightWithoutNearbyTrackingFraudRule>();
builder.Services.AddScoped<IFraudRule, ProductAreaMismatchFraudRule>();

// =========================
// DOMAIN SERVICES
// =========================
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IProductCrudService, ProductCrudService>();
builder.Services.AddScoped<IEventIngestService, EventIngestService>();
builder.Services.AddScoped<IReconciliationService, ReconciliationService>();


// =========================
// FEATURE FLAGS
// =========================
var enableWorkers = configuration.GetValue<bool>("Features:EnableWorkers");

// =========================
// WORKERS (CONTROLADOS)
// =========================
if (enableWorkers)
{
    builder.Services.AddHostedService<CameraTrackingWorker>();
    builder.Services.AddHostedService<WeightSensorWorker>();
}

// =========================
// AUTH
// =========================
builder.Services.AddAuthorization();

// =========================
// BUILD
// =========================
var app = builder.Build();



// =========================
// MIDDLEWARE
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// =========================
// START
// =========================
app.Run();