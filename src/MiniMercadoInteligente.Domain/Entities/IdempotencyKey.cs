namespace MiniMercadoInteligente.Domain.Entities;

public class IdempotencyKey
{
    public Guid IdempotencyKeyId { get; set; }
    public string Key { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string Path { get; set; } = default!;
    public int StatusCode { get; set; }
    public string ContentType { get; set; } = "application/json";
    public string ResponseBody { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);
}
