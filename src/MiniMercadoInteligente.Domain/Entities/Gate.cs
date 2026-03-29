namespace MiniMercadoInteligente.Domain.Entities;

public class Gate
{
    public Guid Id { get; set; }

    public string GateId { get; set; } = default!;
    public string Name { get; set; } = default!;

    public Guid CondominiumId { get; set; }

    public bool IsEntryGate { get; set; } = false;
    public bool IsExitGate { get; set; } = false;

    public bool Active { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}