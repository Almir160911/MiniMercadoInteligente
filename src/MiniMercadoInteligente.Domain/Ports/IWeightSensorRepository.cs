using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IWeightSensorRepository
{
    Task<WeightSensor?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<WeightSensor?> GetBySensorIdAsync(string sensorId, CancellationToken ct = default);

    Task<List<WeightSensor>> GetActiveAsync(CancellationToken ct = default);

    Task<List<WeightSensor>> ListByAreaAsync(string areaCode, CancellationToken ct = default);

    Task<WeightSensor> AddAsync(WeightSensor sensor, CancellationToken ct = default);

    Task<WeightSensor> UpdateAsync(WeightSensor sensor, CancellationToken ct = default);

    Task UpdateReadingAsync(
        string sensorId,
        decimal previousWeight,
        decimal currentWeight,
        DateTime readingAtUtc,
        CancellationToken ct = default);
}