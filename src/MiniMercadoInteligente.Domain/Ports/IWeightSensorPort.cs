namespace MiniMercadoInteligente.Domain.Ports;

public interface IWeightSensorPort
{
    Task<decimal> ReadWeightAsync(string sensorId, CancellationToken ct = default);
}