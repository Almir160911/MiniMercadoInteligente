using System.Text.Json;

namespace MiniMercadoInteligente.Domain.Entities;

public class EventRecord
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = default!;
    public Guid? SessionId { get; set; }
    public Guid? AreaId { get; set; }
    public Guid? ProductId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string Source { get; set; } = default!;
    public string PayloadJson { get; set; } = "{}";

    public static EventRecord ForSessionStarted(Guid sessionId, string deviceId) =>
        new()
        {
            EventId = Guid.NewGuid(),
            EventType = "SESSION_STARTED",
            SessionId = sessionId,
            OccurredAt = DateTime.UtcNow,
            Source = "Totem",
            PayloadJson = JsonSerializer.Serialize(new { deviceId })
        };

    public static EventRecord ForSessionClosed(Guid sessionId, object reconciliation) =>
        new()
        {
            EventId = Guid.NewGuid(),
            EventType = "SESSION_CLOSED",
            SessionId = sessionId,
            OccurredAt = DateTime.UtcNow,
            Source = "Backend",
            PayloadJson = JsonSerializer.Serialize(reconciliation)
        };
}