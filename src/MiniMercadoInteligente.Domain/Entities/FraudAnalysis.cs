namespace MiniMercadoInteligente.Domain.Entities;

public enum FraudSeverity
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public class FraudAnalysis
{
    public Guid FraudAnalysisId { get; set; }
    public Guid SessionId { get; set; }
    public int Score { get; set; }
    public FraudSeverity Severity { get; set; }
    public bool ShouldBlockSession { get; set; }
    public bool ShouldRequireManualReview { get; set; }
    public string FlagsJson { get; set; } = "[]";
    public string NotesJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}