using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IPaymentRepository
{
    Task<Payment?> GetLastBySessionAsync(Guid sessionId, CancellationToken ct);
    Task AddAsync(Payment payment, CancellationToken ct);
    Task<Payment?> FindByWebhookAsync(Guid? paymentId, string? gatewayRef, CancellationToken ct);
    Task UpdateAsync(Payment payment, CancellationToken ct);
}
