namespace MiniMercadoInteligente.Application.Services;

public interface IAutoCheckoutService
{
    Task CheckoutAsync(
        Guid sessionId,
        Guid residentId,
        CancellationToken ct = default);
}