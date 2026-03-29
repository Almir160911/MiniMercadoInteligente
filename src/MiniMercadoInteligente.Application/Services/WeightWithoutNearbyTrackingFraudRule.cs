namespace MiniMercadoInteligente.Application.Services;

public class WeightWithoutNearbyTrackingFraudRule : IFraudRule
{
    public string Name => "WeightWithoutNearbyTracking";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var flags = new List<FraudFlag>();
        var windowSeconds = 8;

        var weightEvents = context.Events
            .Where(e => e.EventType == "WEIGHT_DECREASE" && !string.IsNullOrWhiteSpace(e.Source))
            .ToList();

        foreach (var weightEvent in weightEvents)
        {
            var sourceArea = weightEvent.Source;
            var hasNearbyTrack = context.TrackingSnapshots.Any(t =>
                t.AreaCode == sourceArea &&
                Math.Abs((t.CapturedAtUtc - weightEvent.OccurredAt).TotalSeconds) <= windowSeconds);

            if (!hasNearbyTrack)
            {
                flags.Add(new FraudFlag(
                    "WEIGHT_WITHOUT_TRACKING",
                    $"Mudança de peso sem tracking próximo na área {sourceArea}.",
                    25,
                    "MEDIUM",
                    new
                    {
                        sourceArea,
                        occurredAt = weightEvent.OccurredAt
                    }));
            }
        }

        return Task.FromResult<IReadOnlyList<FraudFlag>>(flags);
    }
}