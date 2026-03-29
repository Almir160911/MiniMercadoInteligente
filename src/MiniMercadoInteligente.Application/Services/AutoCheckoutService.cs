using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Application.Services;

public class AutoCheckoutService : IAutoCheckoutService
{
    private readonly ICartRepository _cart;
    private readonly IPaymentRepository _payments;

    public AutoCheckoutService(
        ICartRepository cart,
        IPaymentRepository payments)
    {
        _cart = cart;
        _payments = payments;
    }

    public async Task CheckoutAsync(
        Guid sessionId,
        Guid residentId,
        CancellationToken ct = default)
    {
        var total = await _cart.GetSessionTotalAsync(sessionId, ct);

        if (total <= 0)
            return;

        var payment = await _payments.CreateAuthorizedChargeAsync(
            sessionId,
            residentId,
            total,
            null,
            ct);

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;

        await _payments.UpdateAsync(payment, ct);

        await _cart.MarkAsCheckedOutAsync(sessionId, ct);
    }
}