using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReconciliationService, ReconciliationService>();
        services.AddScoped<IFraudEngineService, FraudEngineService>();

        services.AddScoped<IFraudRule, QuantityDivergenceFraudRule>();
        services.AddScoped<IFraudRule, ExitWithoutPaymentFraudRule>();
        services.AddScoped<IFraudRule, WeightEventWithoutCartFraudRule>();
        services.AddScoped<IFraudRule, BurstEventFraudRule>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<IEventIngestService, EventIngestService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IProductCrudService, ProductCrudService>();

        return services;
    }
}