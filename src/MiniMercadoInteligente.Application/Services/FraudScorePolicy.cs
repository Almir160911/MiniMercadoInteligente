namespace MiniMercadoInteligente.Application.Services;

public static class FraudScorePolicy
{
    public static FraudDecision Decide(int score)
    {
        if (score >= 60) return FraudDecision.Block;
        if (score >= 30) return FraudDecision.Alert;
        return FraudDecision.Allow;
    }
}