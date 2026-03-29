using MiniMercadoInteligente.Domain.Events;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class EventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : DomainEvent
    {
        return Task.CompletedTask;
    }
}