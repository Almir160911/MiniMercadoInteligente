using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ITrackingRepository
{
    Task AddAsync(TrackingSnapshot snapshot, CancellationToken ct = default);

    Task<TrackingSnapshot?> GetLatestByTrackAsync(string trackId, CancellationToken ct = default);

    Task<List<TrackingSnapshot>> ListByTrackAsync(string trackId, CancellationToken ct = default);

    Task<List<TrackingSnapshot>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default);

    Task<List<TrackingSnapshot>> GetActiveByAreaAsync(string areaCode, CancellationToken ct = default);

    Task UpdateSessionBindingAsync(
        string trackId,
        Guid sessionId,
        CancellationToken ct = default);
}