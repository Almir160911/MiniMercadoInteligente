using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ISessionRepository
{
    Task<Session?> GetAsync(Guid sessionId, CancellationToken ct);
    Task<Session> AddAsync(Session session, CancellationToken ct);
    Task<Session> UpdateAsync(Session session, CancellationToken ct);
    Task<List<Session>> ListAsync(DateTime? from, DateTime? to, string? status, CancellationToken ct);

    Task<Session?> GetOpenByResidentAsync(Guid residentId, CancellationToken ct);
    Task<Session?> GetOpenByDeviceAsync(string deviceId, CancellationToken ct);
}