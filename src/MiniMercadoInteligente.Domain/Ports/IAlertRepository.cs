using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IAlertRepository
{
    Task<Alert> AddAsync(Alert alert, CancellationToken ct);
    Task<Alert> UpdateAsync(Alert alert, CancellationToken ct);
    Task<Alert?> GetAsync(Guid alertId, CancellationToken ct);
    Task<List<Alert>> ListBySessionAsync(Guid sessionId, CancellationToken ct);
    Task<List<Alert>> ListAsync(string? type, string? status, DateTime? from, DateTime? to, CancellationToken ct);
}