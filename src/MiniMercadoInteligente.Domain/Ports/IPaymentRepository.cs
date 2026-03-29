using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken ct = default);

    Task<Payment?> GetLastBySessionAsync(Guid sessionId, CancellationToken ct);

    Task<Payment?> GetLastPendingBySessionAsync(Guid sessionId, CancellationToken ct = default);

    Task<List<Payment>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default);

    Task<Payment?> GetLastByResidentAsync(Guid residentId, CancellationToken ct = default);

    Task AddAsync(Payment payment, CancellationToken ct);

    Task UpdateAsync(Payment payment, CancellationToken ct);

    Task<Payment?> FindByWebhookAsync(Guid? paymentId, string? gatewayRef, CancellationToken ct);

    Task<Payment> CreateAuthorizedChargeAsync(
        Guid sessionId,
        Guid residentId,
        decimal amount,
        string? paymentMethodToken,
        CancellationToken ct = default);

    Task IncrementRetryCountAsync(Guid paymentId, CancellationToken ct = default);

    Task MarkRequiresReviewAsync(Guid paymentId, string reason, CancellationToken ct = default);

    Task<List<Payment>> ListPendingAutoCheckoutAsync(CancellationToken ct = default);
}