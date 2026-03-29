using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IShelfInteractionRepository
{
    Task AddAsync(ShelfInteraction interaction, CancellationToken ct = default);

    Task<ShelfInteraction?> GetAsync(Guid interactionId, CancellationToken ct = default);

    Task<List<ShelfInteraction>> ListBySessionAsync(Guid sessionId, CancellationToken ct = default);

    Task<List<ShelfInteraction>> ListByAreaAsync(string areaCode, CancellationToken ct = default);

    Task<List<ShelfInteraction>> ListPendingReconciliationAsync(CancellationToken ct = default);

    Task UpdateAsync(ShelfInteraction interaction, CancellationToken ct = default);

    Task MarkAsReconciledAsync(Guid interactionId, CancellationToken ct = default);
}