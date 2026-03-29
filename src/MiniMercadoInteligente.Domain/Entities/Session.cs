namespace MiniMercadoInteligente.Domain.Entities;

public enum SessionStatus
{
    Open = 0,
    Paid = 1,
    Closed = 2,
    Cancelled = 3,
    Blocked = 4
}

public class Session
{
    public Guid SessionId { get; set; }
    public Guid ResidentId { get; set; }
    public Guid CondominiumId { get; set; }

    public string DeviceId { get; set; } = default!;

    public SessionStatus Status { get; set; } = SessionStatus.Open;

    // Ciclo lógico da sessão
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    // Ciclo físico da loja
    public DateTime EnteredAtUtc { get; set; }
    public DateTime? ExitedAtUtc { get; set; }

    public string EntryMethod { get; set; } = "APP_QRCODE";
    public string? EntryGateId { get; set; }
    public string? ExitGateId { get; set; }

    // Tracking físico
    public string? CurrentAreaCode { get; set; }
    public string? ActiveTrackId { get; set; }
    public bool TrackingLocked { get; set; } = false;
    public decimal TrackingConfidence { get; set; } = 0m;

    // Checkout automático
    public bool AutoCheckoutTriggered { get; set; } = false;
    public DateTime? AutoCheckoutAtUtc { get; set; }

    // Antifraude
    public bool FraudSuspected { get; set; } = false;
    public int FraudScore { get; set; } = 0;
    public string FraudFlagsJson { get; set; } = "[]";
    public string? BlockReason { get; set; }

    // Auditoria / reconciliação
    public string? ReconciliationVersion { get; set; }

    // Snapshot opcional do meio de pagamento
    public string? DefaultPaymentMethodSnapshot { get; set; }
    
}