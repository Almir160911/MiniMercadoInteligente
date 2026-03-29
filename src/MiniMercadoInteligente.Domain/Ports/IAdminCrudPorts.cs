using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IPlanogramAreaProductRepository
{
    Task<List<PlanogramAreaProduct>> ListAsync(CancellationToken ct = default);
    Task<PlanogramAreaProduct?> GetAsync(Guid areaId, Guid productId, CancellationToken ct = default);
    Task<PlanogramAreaProduct> UpsertAsync(PlanogramAreaProduct entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid areaId, Guid productId, CancellationToken ct = default);
}