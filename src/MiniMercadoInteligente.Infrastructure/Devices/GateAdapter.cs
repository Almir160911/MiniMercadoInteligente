using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Infrastructure.Devices;

public class GateAdapter : IGatePort
{
    public Task OpenAsync(string gateId, CancellationToken ct = default)
    {
        Console.WriteLine($"Gate {gateId} aberto.");
        return Task.CompletedTask;
    }

    public Task CloseAsync(string gateId, CancellationToken ct = default)
    {
        Console.WriteLine($"Gate {gateId} fechado.");
        return Task.CompletedTask;
    }
}