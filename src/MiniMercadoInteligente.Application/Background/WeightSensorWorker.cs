using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniMercadoInteligente.Application.Services;
using MiniMercadoInteligente.Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace MiniMercadoInteligente.Application.Background;

public class WeightSensorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WeightSensorWorker> _logger;

    public WeightSensorWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<WeightSensorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeightSensorWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var sensorRepository = scope.ServiceProvider.GetRequiredService<IWeightSensorRepository>();
                var sensorPort = scope.ServiceProvider.GetRequiredService<IWeightSensorPort>();
                var fusionService = scope.ServiceProvider.GetRequiredService<ISensorFusionService>();

                var sensors = await sensorRepository.GetActiveAsync(stoppingToken);

                foreach (var sensor in sensors)
                {
                    try
                    {
                        var previousWeight = sensor.CurrentWeightGrams;
                        var currentWeight = await sensorPort.ReadWeightAsync(sensor.SensorId, stoppingToken);

                        var delta = Math.Abs(previousWeight - currentWeight);

                        if (delta >= sensor.ToleranceGrams)
                        {
                            var nearestTrackId = "track-nearest-placeholder";

                            await fusionService.ProcessWeightChangeAsync(
                                sensor.SensorId,
                                sensor.AreaCode,
                                previousWeight,
                                currentWeight,
                                nearestTrackId,
                                stoppingToken);

                            await sensorRepository.UpdateReadingAsync(
                                sensor.SensorId,
                                previousWeight,
                                currentWeight,
                                DateTime.UtcNow,
                                stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao processar sensor {SensorId}.",
                            sensor.SensorId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro geral no WeightSensorWorker.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);
        }

        _logger.LogInformation("WeightSensorWorker finalizado.");
    }
}