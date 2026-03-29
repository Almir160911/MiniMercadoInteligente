namespace MiniMercadoInteligente.Application.Contracts;

#region Product

public record ProductAdminResponse(
    Guid ProductId,
    string Sku,
    string Name,
    string Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    decimal CurrentPrice,
    decimal UnitWeightGrams,
    bool IsSoldByUnit,
    bool Active
);

public record CreateProductAdminRequest(
    string Sku,
    string Name,
    string Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    decimal CurrentPrice,
    decimal UnitWeightGrams,
    bool IsSoldByUnit,
    bool Active
);

public record UpdateProductAdminRequest(
    string Name,
    string Barcode,
    string? QrCode,
    int? NominalWeightGrams,
    int WeightToleranceGrams,
    bool IsWeightControlled,
    decimal CurrentPrice,
    decimal UnitWeightGrams,
    bool IsSoldByUnit,
    bool Active
);

#endregion

#region ProductPrice

public record ProductPriceAdminResponse(
    Guid ProductPriceId,
    Guid ProductId,
    decimal Price,
    DateTime EffectiveAtUtc
);

public record SetProductPriceAdminRequest(
    decimal Price,
    DateTime EffectiveAtUtc
);

#endregion

#region Area

public record AreaAdminResponse(
    Guid AreaId,
    string AreaCode,
    string Name,
    string CameraId,
    string SensorId,
    double ConfidenceBase,
    string Type
);

public record CreateAreaAdminRequest(
    string AreaCode,
    string Name,
    string CameraId,
    string SensorId,
    double ConfidenceBase,
    string Type
);

public record UpdateAreaAdminRequest(
    string Name,
    string CameraId,
    string SensorId,
    double ConfidenceBase,
    string Type
);

#endregion

#region Planogram

public record PlanogramAreaProductAdminResponse(
    Guid AreaId,
    Guid ProductId
);

public record UpsertPlanogramAreaProductAdminRequest(
    Guid AreaId,
    Guid ProductId
);

#endregion

#region CameraDevice

public record CameraDeviceAdminResponse(
    Guid Id,
    string CameraId,
    string Name,
    Guid CondominiumId,
    Guid? AreaId,
    string AreaCode,
    string? RtspUrl,
    bool IsEntryCamera,
    bool IsExitCamera,
    bool Active
);

public record CreateCameraDeviceAdminRequest(
    string CameraId,
    string Name,
    Guid CondominiumId,
    Guid? AreaId,
    string AreaCode,
    string? RtspUrl,
    bool IsEntryCamera,
    bool IsExitCamera,
    bool Active
);

public record UpdateCameraDeviceAdminRequest(
    string Name,
    Guid? AreaId,
    string AreaCode,
    string? RtspUrl,
    bool IsEntryCamera,
    bool IsExitCamera,
    bool Active
);

#endregion

#region WeightSensor

public record WeightSensorAdminResponse(
    Guid Id,
    string SensorId,
    string Name,
    Guid CondominiumId,
    Guid? AreaId,
    string AreaCode,
    decimal CurrentWeightGrams,
    decimal LastWeightGrams,
    decimal ToleranceGrams,
    bool Active
);

public record CreateWeightSensorAdminRequest(
    string SensorId,
    string Name,
    Guid CondominiumId,
    Guid? AreaId,
    string AreaCode,
    decimal CurrentWeightGrams,
    decimal LastWeightGrams,
    decimal ToleranceGrams,
    bool Active
);

public record UpdateWeightSensorAdminRequest(
    string Name,
    Guid? AreaId,
    string AreaCode,
    decimal ToleranceGrams,
    bool Active
);

#endregion

#region Gate

public record GateAdminResponse(
    Guid Id,
    string GateId,
    string Name,
    Guid CondominiumId,
    bool IsEntryGate,
    bool IsExitGate,
    bool Active
);

public record CreateGateAdminRequest(
    string GateId,
    string Name,
    Guid CondominiumId,
    bool IsEntryGate,
    bool IsExitGate,
    bool Active
);

public record UpdateGateAdminRequest(
    string Name,
    bool IsEntryGate,
    bool IsExitGate,
    bool Active
);

#endregion

#region DeviceApiKey

public record DeviceApiKeyAdminResponse(
    Guid DeviceApiKeyId,
    string DeviceId,
    string DeviceType,
    bool Active
);

public record CreateOrUpdateDeviceApiKeyAdminRequest(
    string DeviceId,
    string DeviceType,
    string ApiKeyPlain,
    bool Active
);

#endregion