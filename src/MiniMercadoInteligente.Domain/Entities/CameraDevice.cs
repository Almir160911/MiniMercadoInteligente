namespace MiniMercadoInteligente.Domain.Entities;

public class CameraDevice
{
    public Guid Id { get; set; }

    public string CameraId { get; set; } = default!;
    public string Name { get; set; } = default!;

    public Guid CondominiumId { get; set; }

    public Guid? AreaId { get; set; }
    public string AreaCode { get; set; } = default!;

    public string? RtspUrl { get; set; }

    public bool IsEntryCamera { get; set; } = false;
    public bool IsExitCamera { get; set; } = false;

    public bool Active { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}