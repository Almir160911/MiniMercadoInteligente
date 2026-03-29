using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class EventStoreRepository : IEventStore
{
    private readonly AppDbContext _context;

    public EventStoreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AppendAsync(EventRecord record, CancellationToken ct)
    {
        _context.EventRecords.Add(record);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EventRecord>> GetBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        return await _context.EventRecords
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);
    }
}