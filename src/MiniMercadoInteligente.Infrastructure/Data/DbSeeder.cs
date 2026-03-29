using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        await context.Database.MigrateAsync(ct);

        var condominiumId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // =========================
        // AREAS
        // =========================
        if (!await context.Areas.AnyAsync(ct))
        {
            var areaEntrada = new Area
            {
                AreaId = Guid.NewGuid(),
                AreaCode = "ENTRADA",
                Name = "Entrada",
                CameraId = "cam-entrada",
                SensorId = "sensor-1",
                ConfidenceBase = 0.95,
                Type = "ENTRY"
            };

            var areaBebidas = new Area
            {
                AreaId = Guid.NewGuid(),
                AreaCode = "AREA-1",
                Name = "Bebidas",
                CameraId = "cam-area-1",
                SensorId = "sensor-1",
                ConfidenceBase = 0.90,
                Type = "SHELF"
            };

            var areaSaida = new Area
            {
                AreaId = Guid.NewGuid(),
                AreaCode = "SAIDA",
                Name = "Saída",
                CameraId = "cam-area-1",
                SensorId = "sensor-1",
                ConfidenceBase = 0.95,
                Type = "EXIT"
            };

            context.Areas.AddRange(areaEntrada, areaBebidas, areaSaida);
            await context.SaveChangesAsync(ct);
        }

        var area1 = await context.Areas.FirstAsync(x => x.AreaCode == "AREA-1", ct);

    // =========================
    // PRODUCTS
    // =========================
    if (!await context.Products.AnyAsync(ct))
    {
        var coca = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = "COCA001",
            Name = "Coca-Cola Lata 350ml",
            AreaCode = "AREA-1",
            Barcode = "7894900011517",
            QrCode = "QRCODE-COCA001",
            NominalWeightGrams = 365,
            WeightToleranceGrams = 20,
            IsWeightControlled = true,
            CurrentPrice = 6.50m,
            UnitWeightGrams = 365m,
            IsSoldByUnit = true,
            DetectionConfidenceThreshold = 0.85m,
            IsHighRisk = false,
            VisionLabel = "coca_cola_lata_350",
            Active = true
        };

        var agua = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = "AGUA001",
            Name = "Água Mineral 500ml",
            AreaCode = "AREA-1",
            Barcode = "7891000100103",
            QrCode = "QRCODE-AGUA001",
            NominalWeightGrams = 510,
            WeightToleranceGrams = 20,
            IsWeightControlled = true,
            CurrentPrice = 3.50m,
            UnitWeightGrams = 510m,
            IsSoldByUnit = true,
            DetectionConfidenceThreshold = 0.85m,
            IsHighRisk = false,
            VisionLabel = "agua_mineral_500",
            Active = true
        };

        var energetico = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = "ENERG001",
            Name = "Energético 269ml",
            AreaCode = "AREA-1",
            Barcode = "7896045500012",
            QrCode = "QRCODE-ENERG001",
            NominalWeightGrams = 285,
            WeightToleranceGrams = 20,
            IsWeightControlled = true,
            CurrentPrice = 9.90m,
            UnitWeightGrams = 285m,
            IsSoldByUnit = true,
            DetectionConfidenceThreshold = 0.85m,
            IsHighRisk = false,
            VisionLabel = "energetico_269",
            Active = true
        };

        context.Products.AddRange(coca, agua, energetico);
        await context.SaveChangesAsync(ct);

        context.PlanogramAreaProducts.AddRange(
            new PlanogramAreaProduct { AreaId = area1.AreaId, ProductId = coca.ProductId },
            new PlanogramAreaProduct { AreaId = area1.AreaId, ProductId = agua.ProductId },
            new PlanogramAreaProduct { AreaId = area1.AreaId, ProductId = energetico.ProductId }
        );

        await context.SaveChangesAsync(ct);
    }

        // =========================
        // GATES
        // =========================
        if (!await context.Gates.AnyAsync(ct))
        {
            context.Gates.AddRange(
                new Gate
                {
                    Id = Guid.NewGuid(),
                    GateId = "gate-entrada",
                    Name = "Portão Entrada",
                    CondominiumId = condominiumId,
                    IsEntryGate = true,
                    IsExitGate = false,
                    Active = true
                },
                new Gate
                {
                    Id = Guid.NewGuid(),
                    GateId = "gate-saida",
                    Name = "Portão Saída",
                    CondominiumId = condominiumId,
                    IsEntryGate = false,
                    IsExitGate = true,
                    Active = true
                }
            );

            await context.SaveChangesAsync(ct);
        }

        // =========================
        // CAMERAS
        // =========================
        if (!await context.CameraDevices.AnyAsync(ct))
        {
            context.CameraDevices.AddRange(
                new CameraDevice
                {
                    Id = Guid.NewGuid(),
                    CameraId = "cam-entrada",
                    Name = "Câmera Entrada",
                    CondominiumId = condominiumId,
                    AreaId = area1.AreaId,
                    AreaCode = "ENTRADA",
                    RtspUrl = "rtsp://localhost/entrada",
                    IsEntryCamera = true,
                    IsExitCamera = false,
                    Active = true
                },
                new CameraDevice
                {
                    Id = Guid.NewGuid(),
                    CameraId = "cam-area-1",
                    Name = "Câmera Bebidas",
                    CondominiumId = condominiumId,
                    AreaId = area1.AreaId,
                    AreaCode = "AREA-1",
                    RtspUrl = "rtsp://localhost/area1",
                    IsEntryCamera = false,
                    IsExitCamera = false,
                    Active = true
                }
            );

            await context.SaveChangesAsync(ct);
        }

        // =========================
        // SENSORS
        // =========================
        if (!await context.WeightSensors.AnyAsync(ct))
        {
            context.WeightSensors.Add(
                new WeightSensor
                {
                    Id = Guid.NewGuid(),
                    SensorId = "sensor-1",
                    Name = "Sensor Prateleira",
                    CondominiumId = condominiumId,
                    AreaId = area1.AreaId,
                    AreaCode = "AREA-1",
                    CurrentWeightGrams = 2000m,
                    LastWeightGrams = 2000m,
                    ToleranceGrams = 20m,
                    Active = true,
                    LastReadingAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync(ct);
        }

        // =========================
        // DEVICE API KEY
        // =========================
        if (!await context.DeviceApiKeys.AnyAsync(ct))
        {
            context.DeviceApiKeys.Add(
                new DeviceApiKey
                {
                    DeviceApiKeyId = Guid.NewGuid(),
                    DeviceId = "totem-1",
                    DeviceType = "Totem",
                    ApiKeyHash = "SEED-KEY",
                    Active = true
                }
            );

            await context.SaveChangesAsync(ct);
        }
    }
}