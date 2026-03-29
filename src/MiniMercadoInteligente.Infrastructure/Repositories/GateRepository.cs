using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class GateRepository : IGateRepository
{
    private readonly AppDbContext _context;

    public GateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Gate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Gates.FindAsync([id], ct);

    public async Task<Gate?> GetByGateIdAsync(string gateId, CancellationToken ct = default)
        => await _context.Gates.FirstOrDefaultAsync(x => x.GateId == gateId, ct);

    public async Task<List<Gate>> GetActiveAsync(CancellationToken ct = default)
        => await _context.Gates.Where(x => x.Active).ToListAsync(ct);

    public async Task<Gate?> GetEntryGateAsync(CancellationToken ct = default)
        => await _context.Gates.FirstOrDefaultAsync(x => x.IsEntryGate, ct);

    public async Task<Gate?> GetExitGateAsync(CancellationToken ct = default)
        => await _context.Gates.FirstOrDefaultAsync(x => x.IsExitGate, ct);

    public async Task<Gate> AddAsync(Gate gate, CancellationToken ct = default)
    {
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync(ct);
        return gate;
    }

    public async Task<Gate> UpdateAsync(Gate gate, CancellationToken ct = default)
    {
        _context.Gates.Update(gate);
        await _context.SaveChangesAsync(ct);
        return gate;
    }
}