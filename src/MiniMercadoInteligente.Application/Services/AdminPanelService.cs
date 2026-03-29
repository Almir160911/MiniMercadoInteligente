using System.Security.Cryptography;
using System.Text;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public interface IAdminPanelService
{
    Task<List<ProductAdminResponse>> ListProductsAsync(bool? active, CancellationToken ct);
    Task<ProductAdminResponse?> GetProductAsync(Guid productId, CancellationToken ct);
    Task<ProductAdminResponse> CreateProductAsync(CreateProductAdminRequest req, CancellationToken ct);
    Task<ProductAdminResponse> UpdateProductAsync(Guid productId, UpdateProductAdminRequest req, CancellationToken ct);
    Task<bool> DeleteProductAsync(Guid productId, CancellationToken ct);
    Task<ProductPriceAdminResponse> SetProductPriceAsync(Guid productId, SetProductPriceAdminRequest req, CancellationToken ct);

    Task<List<AreaAdminResponse>> ListAreasAsync(CancellationToken ct);
    Task<AreaAdminResponse?> GetAreaAsync(Guid areaId, CancellationToken ct);
    Task<AreaAdminResponse> CreateAreaAsync(CreateAreaAdminRequest req, CancellationToken ct);
    Task<AreaAdminResponse> UpdateAreaAsync(Guid areaId, UpdateAreaAdminRequest req, CancellationToken ct);
    Task<bool> DeleteAreaAsync(Guid areaId, CancellationToken ct);

    Task<List<PlanogramAreaProductAdminResponse>> ListPlanogramAsync(CancellationToken ct);
    Task<PlanogramAreaProductAdminResponse> UpsertPlanogramAsync(UpsertPlanogramAreaProductAdminRequest req, CancellationToken ct);
    Task<bool> DeletePlanogramAsync(Guid areaId, Guid productId, CancellationToken ct);

    Task<List<CameraDeviceAdminResponse>> ListCamerasAsync(CancellationToken ct);
    Task<CameraDeviceAdminResponse?> GetCameraAsync(Guid id, CancellationToken ct);
    Task<CameraDeviceAdminResponse> CreateCameraAsync(CreateCameraDeviceAdminRequest req, CancellationToken ct);
    Task<CameraDeviceAdminResponse> UpdateCameraAsync(Guid id, UpdateCameraDeviceAdminRequest req, CancellationToken ct);
    Task<bool> DeleteCameraAsync(Guid id, CancellationToken ct);

    Task<List<WeightSensorAdminResponse>> ListSensorsAsync(CancellationToken ct);
    Task<WeightSensorAdminResponse?> GetSensorAsync(Guid id, CancellationToken ct);
    Task<WeightSensorAdminResponse> CreateSensorAsync(CreateWeightSensorAdminRequest req, CancellationToken ct);
    Task<WeightSensorAdminResponse> UpdateSensorAsync(Guid id, UpdateWeightSensorAdminRequest req, CancellationToken ct);
    Task<bool> DeleteSensorAsync(Guid id, CancellationToken ct);

    Task<List<GateAdminResponse>> ListGatesAsync(CancellationToken ct);
    Task<GateAdminResponse?> GetGateAsync(Guid id, CancellationToken ct);
    Task<GateAdminResponse> CreateGateAsync(CreateGateAdminRequest req, CancellationToken ct);
    Task<GateAdminResponse> UpdateGateAsync(Guid id, UpdateGateAdminRequest req, CancellationToken ct);
    Task<bool> DeleteGateAsync(Guid id, CancellationToken ct);

    Task<DeviceApiKeyAdminResponse> UpsertDeviceApiKeyAsync(CreateOrUpdateDeviceApiKeyAdminRequest req, CancellationToken ct);
    Task<bool> DeleteDeviceApiKeyAsync(Guid deviceApiKeyId, CancellationToken ct);
}

public class AdminPanelService : IAdminPanelService
{
    private readonly IProductCatalogRepository _products;
    private readonly IAreaRepository _areas;
    private readonly ICameraRepository _cameras;
    private readonly IWeightSensorRepository _sensors;
    private readonly IGateRepository _gates;
    private readonly IDeviceApiKeyRepository _deviceApiKeys;

    public AdminPanelService(
        IProductCatalogRepository products,
        IAreaRepository areas,
        ICameraRepository cameras,
        IWeightSensorRepository sensors,
        IGateRepository gates,
        IDeviceApiKeyRepository deviceApiKeys)
    {
        _products = products;
        _areas = areas;
        _cameras = cameras;
        _sensors = sensors;
        _gates = gates;
        _deviceApiKeys = deviceApiKeys;
    }
    public async Task<List<ProductAdminResponse>> ListProductsAsync(bool? active, CancellationToken ct)
        => (await _products.ListProductsByStatusAsync(active, ct)).Select(MapProduct).ToList();

    public async Task<ProductAdminResponse?> GetProductAsync(Guid productId, CancellationToken ct)
    {
        var item = await _products.GetByIdAsync(productId, ct);
        return item is null ? null : MapProduct(item);
    }

    public async Task<ProductAdminResponse> CreateProductAsync(CreateProductAdminRequest req, CancellationToken ct)
    {
        var entity = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = req.Sku,
            Name = req.Name,
            Barcode = req.Barcode,
            QrCode = req.QrCode,
            NominalWeightGrams = req.NominalWeightGrams,
            WeightToleranceGrams = req.WeightToleranceGrams,
            IsWeightControlled = req.IsWeightControlled,
            CurrentPrice = req.CurrentPrice,
            UnitWeightGrams = req.UnitWeightGrams,
            IsSoldByUnit = req.IsSoldByUnit,
            Active = req.Active,
            AreaCode = "AREA-1",
            DetectionConfidenceThreshold = 0.85m,
            IsHighRisk = false
        };

        await _products.CreateAsync(entity, ct);
        return MapProduct(entity);
    }

    public async Task<ProductAdminResponse> UpdateProductAsync(Guid productId, UpdateProductAdminRequest req, CancellationToken ct)
    {
        var entity = await _products.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        entity.Name = req.Name;
        entity.Barcode = req.Barcode;
        entity.QrCode = req.QrCode;
        entity.NominalWeightGrams = req.NominalWeightGrams;
        entity.WeightToleranceGrams = req.WeightToleranceGrams;
        entity.IsWeightControlled = req.IsWeightControlled;
        entity.CurrentPrice = req.CurrentPrice;
        entity.UnitWeightGrams = req.UnitWeightGrams;
        entity.IsSoldByUnit = req.IsSoldByUnit;
        entity.Active = req.Active;

        await _products.UpdateAsync(entity, ct);
        return MapProduct(entity);
    }

    public Task<bool> DeleteProductAsync(Guid productId, CancellationToken ct)
        => _products.DeleteAsync(productId, ct);

    public async Task<ProductPriceAdminResponse> SetProductPriceAsync(Guid productId, SetProductPriceAdminRequest req, CancellationToken ct)
    {
        var price = new ProductPrice
        {
            ProductPriceId = Guid.NewGuid(),
            ProductId = productId,
            Price = req.Price
        };

        var saved = await _products.SetPriceAsync(productId, price, ct);

        return new ProductPriceAdminResponse(
            saved.ProductPriceId,
            saved.ProductId,
            saved.Price,
            DateTime.UtcNow
        );
    }

    public async Task<List<AreaAdminResponse>> ListAreasAsync(CancellationToken ct)
        => (await _areas.ListAsync(ct)).Select(MapArea).ToList();

    public async Task<AreaAdminResponse?> GetAreaAsync(Guid areaId, CancellationToken ct)
    {
        var item = await _areas.GetAsync(areaId, ct);
        return item is null ? null : MapArea(item);
    }

    public async Task<AreaAdminResponse> CreateAreaAsync(CreateAreaAdminRequest req, CancellationToken ct)
    {
        var entity = new Area
        {
            AreaId = Guid.NewGuid(),
            AreaCode = req.AreaCode,
            Name = req.Name,
            CameraId = req.CameraId,
            SensorId = req.SensorId,
            ConfidenceBase = req.ConfidenceBase,
            Type = req.Type
        };

        await _areas.AddAsync(entity, ct);
        return MapArea(entity);
    }

    public async Task<AreaAdminResponse> UpdateAreaAsync(Guid areaId, UpdateAreaAdminRequest req, CancellationToken ct)
    {
        var entity = await _areas.GetAsync(areaId, ct)
            ?? throw new InvalidOperationException("Área não encontrada.");

        entity.Name = req.Name;
        entity.CameraId = req.CameraId;
        entity.SensorId = req.SensorId;
        entity.ConfidenceBase = req.ConfidenceBase;
        entity.Type = req.Type;

        await _areas.UpdateAsync(entity, ct);
        return MapArea(entity);
    }

    public Task<bool> DeleteAreaAsync(Guid areaId, CancellationToken ct)
        => _areas.DeleteAsync(areaId, ct);

    public async Task<List<PlanogramAreaProductAdminResponse>> ListPlanogramAsync(CancellationToken ct)
    {
        var areas = await _areas.ListAsync(ct);
        var result = new List<PlanogramAreaProductAdminResponse>();

        foreach (var area in areas)
        {
            var items = await _areas.GetPlanogramAsync(area.AreaId, ct);
            result.AddRange(items.Select(x => new PlanogramAreaProductAdminResponse(x.AreaId, x.ProductId)));
        }

        return result;
    }

    public async Task<PlanogramAreaProductAdminResponse> UpsertPlanogramAsync(UpsertPlanogramAreaProductAdminRequest req, CancellationToken ct)
    {
        var entity = new PlanogramAreaProduct
        {
            AreaId = req.AreaId,
            ProductId = req.ProductId
        };

        var saved = await _areas.UpsertPlanogramAsync(req.AreaId, req.ProductId, entity, ct);
        return new PlanogramAreaProductAdminResponse(saved.AreaId, saved.ProductId);
    }

    public async Task<bool> DeletePlanogramAsync(Guid areaId, Guid productId, CancellationToken ct)
    {
        var items = await _areas.GetPlanogramAsync(areaId, ct);
        var item = items.FirstOrDefault(x => x.AreaId == areaId && x.ProductId == productId);
        if (item is null) return false;

        // como seu IAreaRepository atual não expõe delete do planograma,
        // esta operação fica como "não suportada" até você adicionar no repository.
        return false;
    }

    public async Task<List<CameraDeviceAdminResponse>> ListCamerasAsync(CancellationToken ct)
        => (await _cameras.GetActiveAsync(ct)).Select(MapCamera).ToList();

    public async Task<CameraDeviceAdminResponse?> GetCameraAsync(Guid id, CancellationToken ct)
    {
        var item = await _cameras.GetByIdAsync(id, ct);
        return item is null ? null : MapCamera(item);
    }

    public async Task<CameraDeviceAdminResponse> CreateCameraAsync(CreateCameraDeviceAdminRequest req, CancellationToken ct)
    {
        var entity = new CameraDevice
        {
            Id = Guid.NewGuid(),
            CameraId = req.CameraId,
            Name = req.Name,
            CondominiumId = req.CondominiumId,
            AreaId = req.AreaId,
            AreaCode = req.AreaCode,
            RtspUrl = req.RtspUrl,
            IsEntryCamera = req.IsEntryCamera,
            IsExitCamera = req.IsExitCamera,
            Active = req.Active
        };

        await _cameras.AddAsync(entity, ct);
        return MapCamera(entity);
    }

    public async Task<CameraDeviceAdminResponse> UpdateCameraAsync(Guid id, UpdateCameraDeviceAdminRequest req, CancellationToken ct)
    {
        var entity = await _cameras.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Câmera não encontrada.");

        entity.Name = req.Name;
        entity.AreaId = req.AreaId;
        entity.AreaCode = req.AreaCode;
        entity.RtspUrl = req.RtspUrl;
        entity.IsEntryCamera = req.IsEntryCamera;
        entity.IsExitCamera = req.IsExitCamera;
        entity.Active = req.Active;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _cameras.UpdateAsync(entity, ct);
        return MapCamera(entity);
    }

    public async Task<bool> DeleteCameraAsync(Guid id, CancellationToken ct)
    {
        var entity = await _cameras.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.Active = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _cameras.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<List<WeightSensorAdminResponse>> ListSensorsAsync(CancellationToken ct)
        => (await _sensors.GetActiveAsync(ct)).Select(MapSensor).ToList();

    public async Task<WeightSensorAdminResponse?> GetSensorAsync(Guid id, CancellationToken ct)
    {
        var item = await _sensors.GetByIdAsync(id, ct);
        return item is null ? null : MapSensor(item);
    }

    public async Task<WeightSensorAdminResponse> CreateSensorAsync(CreateWeightSensorAdminRequest req, CancellationToken ct)
    {
        var entity = new WeightSensor
        {
            Id = Guid.NewGuid(),
            SensorId = req.SensorId,
            Name = req.Name,
            CondominiumId = req.CondominiumId,
            AreaId = req.AreaId,
            AreaCode = req.AreaCode,
            CurrentWeightGrams = req.CurrentWeightGrams,
            LastWeightGrams = req.LastWeightGrams,
            ToleranceGrams = req.ToleranceGrams,
            Active = req.Active,
            LastReadingAtUtc = DateTime.UtcNow
        };

        await _sensors.AddAsync(entity, ct);
        return MapSensor(entity);
    }

    public async Task<WeightSensorAdminResponse> UpdateSensorAsync(Guid id, UpdateWeightSensorAdminRequest req, CancellationToken ct)
    {
        var entity = await _sensors.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Sensor não encontrado.");

        entity.Name = req.Name;
        entity.AreaId = req.AreaId;
        entity.AreaCode = req.AreaCode;
        entity.ToleranceGrams = req.ToleranceGrams;
        entity.Active = req.Active;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _sensors.UpdateAsync(entity, ct);
        return MapSensor(entity);
    }

    public async Task<bool> DeleteSensorAsync(Guid id, CancellationToken ct)
    {
        var entity = await _sensors.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.Active = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _sensors.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<List<GateAdminResponse>> ListGatesAsync(CancellationToken ct)
        => (await _gates.GetActiveAsync(ct)).Select(MapGate).ToList();

    public async Task<GateAdminResponse?> GetGateAsync(Guid id, CancellationToken ct)
    {
        var item = await _gates.GetByIdAsync(id, ct);
        return item is null ? null : MapGate(item);
    }

    public async Task<GateAdminResponse> CreateGateAsync(CreateGateAdminRequest req, CancellationToken ct)
    {
        var entity = new Gate
        {
            Id = Guid.NewGuid(),
            GateId = req.GateId,
            Name = req.Name,
            CondominiumId = req.CondominiumId,
            IsEntryGate = req.IsEntryGate,
            IsExitGate = req.IsExitGate,
            Active = req.Active
        };

        await _gates.AddAsync(entity, ct);
        return MapGate(entity);
    }

    public async Task<GateAdminResponse> UpdateGateAsync(Guid id, UpdateGateAdminRequest req, CancellationToken ct)
    {
        var entity = await _gates.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Gate não encontrado.");

        entity.Name = req.Name;
        entity.IsEntryGate = req.IsEntryGate;
        entity.IsExitGate = req.IsExitGate;
        entity.Active = req.Active;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _gates.UpdateAsync(entity, ct);
        return MapGate(entity);
    }

    public async Task<bool> DeleteGateAsync(Guid id, CancellationToken ct)
    {
        var entity = await _gates.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.Active = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _gates.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<DeviceApiKeyAdminResponse> UpsertDeviceApiKeyAsync(CreateOrUpdateDeviceApiKeyAdminRequest req, CancellationToken ct)
    {
        var entity = new DeviceApiKey
        {
            DeviceApiKeyId = Guid.NewGuid(),
            DeviceId = req.DeviceId,
            DeviceType = req.DeviceType,
            ApiKeyHash = Sha256(req.ApiKeyPlain),
            Active = req.Active
        };

        var saved = await _deviceApiKeys.UpsertAsync(entity, ct);

        return new DeviceApiKeyAdminResponse(
            saved.DeviceApiKeyId,
            saved.DeviceId,
            saved.DeviceType,
            saved.Active
        );
    }

    public async Task<bool> DeleteDeviceApiKeyAsync(Guid deviceApiKeyId, CancellationToken ct)
    {
        var entity = await _deviceApiKeys.GetAsync(deviceApiKeyId, ct);
        if (entity is null) return false;

        entity.Active = false;
        await _deviceApiKeys.UpsertAsync(entity, ct);
        return true;
    }

    private static ProductAdminResponse MapProduct(Product p)
        => new(
            p.ProductId,
            p.Sku,
            p.Name,
            p.Barcode,
            p.QrCode,
            p.NominalWeightGrams,
            p.WeightToleranceGrams,
            p.IsWeightControlled,
            p.CurrentPrice,
            p.UnitWeightGrams,
            p.IsSoldByUnit,
            p.Active
        );

    private static AreaAdminResponse MapArea(Area a)
        => new(
            a.AreaId,
            a.AreaCode,
            a.Name,
            a.CameraId,
            a.SensorId,
            a.ConfidenceBase,
            a.Type
        );

    private static CameraDeviceAdminResponse MapCamera(CameraDevice x)
        => new(
            x.Id,
            x.CameraId,
            x.Name,
            x.CondominiumId,
            x.AreaId,
            x.AreaCode,
            x.RtspUrl,
            x.IsEntryCamera,
            x.IsExitCamera,
            x.Active
        );

    private static WeightSensorAdminResponse MapSensor(WeightSensor x)
        => new(
            x.Id,
            x.SensorId,
            x.Name,
            x.CondominiumId,
            x.AreaId,
            x.AreaCode,
            x.CurrentWeightGrams,
            x.LastWeightGrams,
            x.ToleranceGrams,
            x.Active
        );

    private static GateAdminResponse MapGate(Gate x)
        => new(
            x.Id,
            x.GateId,
            x.Name,
            x.CondominiumId,
            x.IsEntryGate,
            x.IsExitGate,
            x.Active
        );

    private static string Sha256(string value)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}