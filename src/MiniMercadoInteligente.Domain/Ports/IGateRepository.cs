using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IGateRepository
{
    Task<Gate?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Gate?> GetByGateIdAsync(string gateId, CancellationToken ct = default);

    Task<List<Gate>> GetActiveAsync(CancellationToken ct = default);

    Task<Gate?> GetEntryGateAsync(CancellationToken ct = default);

    Task<Gate?> GetExitGateAsync(CancellationToken ct = default);

    Task<Gate> AddAsync(Gate gate, CancellationToken ct = default);

    Task<Gate> UpdateAsync(Gate gate, CancellationToken ct = default);
}