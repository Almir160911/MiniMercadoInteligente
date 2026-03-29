using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly AppDbContext _context;

    public AreaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Area>> ListAsync(CancellationToken ct = default)
        => _context.Areas.OrderBy(x => x.AreaCode).ToListAsync(ct);

    public Task<Area?> GetAsync(Guid areaId, CancellationToken ct = default)
        => _context.Areas.FirstOrDefaultAsync(x => x.AreaId == areaId, ct);

    public async Task<Area> AddAsync(Area area, CancellationToken ct = default)
    {
        _context.Areas.Add(area);
        await _context.SaveChangesAsync(ct);
        return area;
    }

    public async Task<Area> UpdateAsync(Area area, CancellationToken ct = default)
    {
        _context.Areas.Update(area);
        await _context.SaveChangesAsync(ct);
        return area;
    }

    public async Task<bool> DeleteAsync(Guid areaId, CancellationToken ct = default)
    {
        var entity = await _context.Areas.FirstOrDefaultAsync(x => x.AreaId == areaId, ct);
        if (entity is null) return false;

        _context.Areas.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<List<PlanogramAreaProduct>> GetPlanogramAsync(Guid areaId, CancellationToken ct = default)
        => _context.PlanogramAreaProducts
            .Where(x => x.AreaId == areaId)
            .ToListAsync(ct);

    public async Task<PlanogramAreaProduct> UpsertPlanogramAsync(
        Guid areaId,
        Guid productId,
        PlanogramAreaProduct entity,
        CancellationToken ct = default)
    {
        var existing = await _context.PlanogramAreaProducts
            .FirstOrDefaultAsync(x => x.AreaId == areaId && x.ProductId == productId, ct);

        if (existing is null)
        {
            _context.PlanogramAreaProducts.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        await _context.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<Guid?> GetFirstActiveProductIdFromPlanogramAsync(Guid areaId, CancellationToken ct = default)
    {
        return await _context.PlanogramAreaProducts
            .Where(p => p.AreaId == areaId)
            .Join(
                _context.Products.Where(prod => prod.Active),
                pap => pap.ProductId,
                prod => prod.ProductId,
                (pap, prod) => (Guid?)prod.ProductId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<decimal?> GetUnitWeightAsync(Guid areaId, CancellationToken ct = default)
    {
        return await _context.PlanogramAreaProducts
            .Where(p => p.AreaId == areaId)
            .Join(
                _context.Products.Where(prod => prod.Active),
                pap => pap.ProductId,
                prod => prod.ProductId,
                (pap, prod) => (decimal?)prod.UnitWeightGrams)
            .FirstOrDefaultAsync(ct);
    }
}