using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IAreaRepository
{
    Task<List<Area>> ListAsync(CancellationToken ct = default);
    Task<Area?> GetAsync(Guid areaId, CancellationToken ct = default);
    Task<Area> AddAsync(Area area, CancellationToken ct = default);
    Task<Area> UpdateAsync(Area area, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid areaId, CancellationToken ct = default);

    Task<List<PlanogramAreaProduct>> GetPlanogramAsync(Guid areaId, CancellationToken ct = default);
    Task<PlanogramAreaProduct> UpsertPlanogramAsync(Guid areaId, Guid productId, PlanogramAreaProduct entity, CancellationToken ct = default);
    Task<Guid?> GetFirstActiveProductIdFromPlanogramAsync(Guid areaId, CancellationToken ct = default);
    Task<decimal?> GetUnitWeightAsync(Guid areaId, CancellationToken ct = default);
}