namespace MiniMercadoInteligente.Domain.Entities;

public enum PaymentStatus
{
    Pending = 0,
    Authorized = 1,
    Paid = 2,
    Failed = 3,
    Cancelled = 4,
    RequiresReview = 5
}

public class Payment
{
    public Guid PaymentId { get; set; }
    public Guid SessionId { get; set; }
    public Guid ResidentId { get; set; }

    public string Method { get; set; } = "Pix";
    public string TriggerSource { get; set; } = "MANUAL"; // MANUAL, AUTO_EXIT, REPROCESS

    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public bool IsAutoCheckout { get; set; } = false;

    public string? PaymentMethodToken { get; set; }
    public string? GatewayRef { get; set; }
    public string? GatewayPayloadJson { get; set; }

    public int RetryCount { get; set; } = 0;
    public string? FailureReason { get; set; }
}