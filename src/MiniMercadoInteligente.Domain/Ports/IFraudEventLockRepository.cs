namespace MiniMercadoInteligente.Domain.Ports;

public interface IFraudEventLockRepository
{
    Task<bool> ExistsAsync(string source, string externalEventId, CancellationToken ct);
    Task SaveAsync(string source, string externalEventId, string hash, CancellationToken ct);
}