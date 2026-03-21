using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IAreaRepository
{
    Task<List<Area>> ListAsync(CancellationToken ct);
    Task<Area> AddAsync(Area area, CancellationToken ct);
    Task<List<PlanogramAreaProduct>> GetPlanogramAsync(Guid areaId, CancellationToken ct);
    Task<PlanogramAreaProduct> UpsertPlanogramAsync(Guid areaId, Guid productId, PlanogramAreaProduct item, CancellationToken ct);
    Task<Guid?> GetFirstActiveProductIdFromPlanogramAsync(Guid areaId, CancellationToken ct);
    Task<int> GetUnitWeightAsync(Guid productId, CancellationToken ct);
}
