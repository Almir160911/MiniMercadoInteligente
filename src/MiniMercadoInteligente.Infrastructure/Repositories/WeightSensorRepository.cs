using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure.Repositories;

public class WeightSensorRepository : IWeightSensorRepository
{
    private readonly AppDbContext _context;

    public WeightSensorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WeightSensor?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.WeightSensors.FindAsync([id], ct);

    public async Task<WeightSensor?> GetBySensorIdAsync(string sensorId, CancellationToken ct = default)
        => await _context.WeightSensors.FirstOrDefaultAsync(x => x.SensorId == sensorId, ct);

    public async Task<List<WeightSensor>> GetActiveAsync(CancellationToken ct = default)
        => await _context.WeightSensors.Where(x => x.Active).ToListAsync(ct);

    public async Task<List<WeightSensor>> ListByAreaAsync(string areaCode, CancellationToken ct = default)
        => await _context.WeightSensors.Where(x => x.AreaCode == areaCode).ToListAsync(ct);

    public async Task<WeightSensor> AddAsync(WeightSensor sensor, CancellationToken ct = default)
    {
        _context.WeightSensors.Add(sensor);
        await _context.SaveChangesAsync(ct);
        return sensor;
    }

    public async Task<WeightSensor> UpdateAsync(WeightSensor sensor, CancellationToken ct = default)
    {
        _context.WeightSensors.Update(sensor);
        await _context.SaveChangesAsync(ct);
        return sensor;
    }

    public async Task UpdateReadingAsync(string sensorId, decimal previous, decimal current, DateTime atUtc, CancellationToken ct = default)
    {
        var sensor = await _context.WeightSensors.FirstOrDefaultAsync(x => x.SensorId == sensorId, ct);
        if (sensor is null) return;

        sensor.LastWeightGrams = previous;
        sensor.CurrentWeightGrams = current;
        sensor.LastReadingAtUtc = atUtc;

        await _context.SaveChangesAsync(ct);
    }
}