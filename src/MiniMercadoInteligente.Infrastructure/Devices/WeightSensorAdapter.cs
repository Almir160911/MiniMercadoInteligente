using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Infrastructure.Devices;

public class WeightSensorAdapter : IWeightSensorPort
{
    public Task<decimal> ReadWeightAsync(string sensorId, CancellationToken ct = default)
    {
        var simulatedWeight = 850.0m;
        return Task.FromResult(simulatedWeight);
    }
}