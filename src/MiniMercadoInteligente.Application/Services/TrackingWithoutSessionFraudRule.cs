namespace MiniMercadoInteligente.Application.Services;

public class TrackingWithoutSessionFraudRule : IFraudRule
{
    public string Name => "TrackingWithoutSession";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var orphanTracks = context.TrackingSnapshots
            .Where(x => x.SessionId == null)
            .ToList();

        if (orphanTracks.Count == 0)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        IReadOnlyList<FraudFlag> flags =
        [
            new FraudFlag(
                "TRACK_WITHOUT_SESSION",
                "Foram detectados rastreamentos sem sessão associada.",
                orphanTracks.Count >= 3 ? 25 : 10,
                orphanTracks.Count >= 3 ? "MEDIUM" : "LOW",
                new
                {
                    count = orphanTracks.Count,
                    areas = orphanTracks.Select(x => x.AreaCode).Distinct().ToArray()
                })
        ];

        return Task.FromResult(flags);
    }
}