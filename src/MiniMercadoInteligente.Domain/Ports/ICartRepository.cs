using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ICartRepository
{
    Task<List<CartItem>> ListBySessionAsync(Guid sessionId, CancellationToken ct);

    Task<CartItem?> FindBySessionAndProductAsync(Guid sessionId, Guid productId, CancellationToken ct = default);

    Task AddAsync(CartItem item, CancellationToken ct);
    Task UpdateAsync(CartItem item, CancellationToken ct = default);
    Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct);

    Task AddOrIncrementItemAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        decimal unitPrice,
        CancellationToken ct = default);

    Task DecrementOrRemoveItemAsync(
        Guid sessionId,
        Guid productId,
        int qty,
        CancellationToken ct = default);

    Task<decimal> GetSessionTotalAsync(Guid sessionId, CancellationToken ct = default);

    Task MarkAsCheckedOutAsync(Guid sessionId, CancellationToken ct = default);
    Task MarkItemsAsReconciledAsync(Guid sessionId, CancellationToken ct = default);
}