using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface ICartRepository
{
    Task<List<CartItem>> ListBySessionAsync(Guid sessionId, CancellationToken ct);
    Task AddAsync(CartItem item, CancellationToken ct);
    Task RemoveAsync(Guid sessionId, Guid itemId, CancellationToken ct);
}
