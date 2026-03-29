using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class ShelfInteractionRepository : IShelfInteractionRepository
{
    private readonly AppDbContext _context;

    public ShelfInteractionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ShelfInteraction interaction, CancellationToken ct = default)
    {
        _context.ShelfInteractions.Add(interaction);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<ShelfInteraction?> GetAsync(Guid interactionId, CancellationToken ct = default)
    {
        return await _context.ShelfInteractions.FindAsync([interactionId], ct);
    }

    public async Task<List<ShelfInteraction>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        return await _context.ShelfInteractions
            .Where(x => x.SessionId == sessionId)
            .ToListAsync(ct);
    }

    public async Task<List<ShelfInteraction>> ListByAreaAsync(string areaCode, CancellationToken ct = default)
    {
        return await _context.ShelfInteractions
            .Where(x => x.AreaCode == areaCode)
            .ToListAsync(ct);
    }

    public async Task<List<ShelfInteraction>> ListPendingReconciliationAsync(CancellationToken ct = default)
    {
        return await _context.ShelfInteractions
            .Where(x => !x.Reconciled)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(ShelfInteraction interaction, CancellationToken ct = default)
    {
        _context.ShelfInteractions.Update(interaction);
        await _context.SaveChangesAsync(ct);
    }

    public async Task MarkAsReconciledAsync(Guid interactionId, CancellationToken ct = default)
    {
        var item = await _context.ShelfInteractions.FindAsync([interactionId], ct);
        if (item is null) return;

        item.Reconciled = true;
        await _context.SaveChangesAsync(ct);
    }
}