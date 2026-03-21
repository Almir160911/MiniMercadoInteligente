using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IEventStore
{
    Task AppendAsync(EventRecord ev, CancellationToken ct);
    Task<List<EventRecord>> GetBySessionAsync(Guid sessionId, CancellationToken ct);
}
