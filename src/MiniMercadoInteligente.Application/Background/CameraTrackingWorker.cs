using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniMercadoInteligente.Application.Services;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Background;

public class CameraTrackingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CameraTrackingWorker> _logger;

    public CameraTrackingWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<CameraTrackingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CameraTrackingWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cameraRepository = scope.ServiceProvider.GetRequiredService<ICameraRepository>();
                var visionPort = scope.ServiceProvider.GetRequiredService<ICameraVisionPort>();
                var trackingService = scope.ServiceProvider.GetRequiredService<ITrackingService>();

                var cameras = await cameraRepository.GetActiveAsync(stoppingToken);

                foreach (var camera in cameras)
                {
                    try
                    {
                        var snapshots = await visionPort.DetectPeopleAsync(camera, stoppingToken);

                        foreach (var snapshot in snapshots)
                        {
                            await trackingService.RegisterTrackingAsync(
                                snapshot.CameraId,
                                snapshot.TrackId,
                                snapshot.AreaCode,
                                snapshot.BoundingBox.X,
                                snapshot.BoundingBox.Y,
                                snapshot.BoundingBox.Width,
                                snapshot.BoundingBox.Height,
                                snapshot.Confidence,
                                stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao processar câmera {CameraId}.",
                            camera.CameraId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro geral no CameraTrackingWorker.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
        }

        _logger.LogInformation("CameraTrackingWorker finalizado.");
    }
}