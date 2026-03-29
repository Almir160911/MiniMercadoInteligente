using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class TrackingRepository : ITrackingRepository
{
    private readonly AppDbContext _context;

    public TrackingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TrackingSnapshot snapshot, CancellationToken ct = default)
    {
        _context.TrackingSnapshots.Add(snapshot);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<TrackingSnapshot?> GetLatestByTrackAsync(string trackId, CancellationToken ct = default)
    {
        return await _context.TrackingSnapshots
            .Where(x => x.TrackId == trackId)
            .OrderByDescending(x => x.CapturedAtUtc)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<TrackingSnapshot>> ListByTrackAsync(string trackId, CancellationToken ct = default)
    {
        return await _context.TrackingSnapshots
            .Where(x => x.TrackId == trackId)
            .ToListAsync(ct);
    }

    public async Task<List<TrackingSnapshot>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.TrackingSnapshots
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);
    }

    public async Task<List<TrackingSnapshot>> GetActiveByAreaAsync(string areaCode, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-5);

        return await _context.TrackingSnapshots
            .Where(x => x.AreaCode == areaCode && x.CapturedAtUtc >= cutoff)
            .ToListAsync(ct);
    }

    public async Task UpdateSessionBindingAsync(string trackId, Guid sessionId, CancellationToken ct = default)
    {
        var snapshots = await _context.TrackingSnapshots
            .Where(x => x.TrackId == trackId)
            .ToListAsync(ct);

        foreach (var s in snapshots)
            s.SessionId = sessionId;

        await _context.SaveChangesAsync(ct);
    }
}