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
    Task<Session?> GetOpenByTrackIdAsync(string trackId, CancellationToken ct = default);

    Task<List<Session>> ListOpenByAreaAsync(string areaCode, CancellationToken ct = default);

    Task<Session> CreateOpenSessionAsync(
        Guid residentId,
        Guid condominiumId,
        string deviceId,
        string entryMethod,
        string? entryGateId,
        CancellationToken ct = default);

    Task CloseSessionAsync(
        Guid sessionId,
        DateTime endedAtUtc,
        DateTime? exitedAtUtc,
        CancellationToken ct = default);

    Task UpdateTrackingAsync(
        Guid sessionId,
        string? activeTrackId,
        string? currentAreaCode,
        decimal trackingConfidence,
        bool trackingLocked,
        CancellationToken ct = default);

    Task MarkAutoCheckoutTriggeredAsync(
        Guid sessionId,
        DateTime triggeredAtUtc,
        CancellationToken ct = default);

    Task BlockSessionAsync(
        Guid sessionId,
        string reason,
        CancellationToken ct = default);
}