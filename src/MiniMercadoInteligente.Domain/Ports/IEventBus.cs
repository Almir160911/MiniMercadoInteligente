using MiniMercadoInteligente.Domain.Events;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : DomainEvent;
}