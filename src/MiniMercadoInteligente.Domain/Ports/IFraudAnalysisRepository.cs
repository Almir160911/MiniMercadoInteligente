using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Domain.Ports;

public interface IFraudAnalysisRepository
{
    Task<FraudAnalysis> AddAsync(FraudAnalysis entity, CancellationToken ct);
    Task<FraudAnalysis?> GetLastBySessionAsync(Guid sessionId, CancellationToken ct);
}