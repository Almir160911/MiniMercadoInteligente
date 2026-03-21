using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Domain.Ports;
using System.Text.Json;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IAlertRepository _alerts;

    public ReportsController(IAlertRepository alerts)
    {
        _alerts = alerts;
    }

    [HttpGet("kpis")]
    public async Task<ActionResult<object>> Kpis(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var list = await _alerts.ListAsync(
            status: null,
            type: null,
            from: from,
            to: to,
            ct: ct
);
        var div = list.Where(a => a.Type == "Divergence").ToList();

        var loss = div.Sum(a => a.EstimatedLoss);
        var open = div.Count(a => a.Status.ToString() == "Open");
        var resolved = div.Count(a => a.Status.ToString() == "Resolved");

        var bySeverity = div.GroupBy(a => a.Severity.ToString())
            .Select(g => new
            {
                severity = g.Key,
                count = g.Count(),
                estimatedLoss = g.Sum(x => x.EstimatedLoss)
            })
            .OrderByDescending(x => x.estimatedLoss)
            .ToList();

        var topSessions = div.GroupBy(a => a.SessionId)
            .Select(g => new
            {
                sessionId = g.Key,
                estimatedLoss = g.Sum(x => x.EstimatedLoss),
                count = g.Count()
            })
            .OrderByDescending(x => x.estimatedLoss)
            .Take(5)
            .ToList();

        var bySku = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var a in div)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a.PayloadJson))
                    continue;

                using var doc = JsonDocument.Parse(a.PayloadJson);

                if (doc.RootElement.TryGetProperty("sku", out var skuEl) &&
                    skuEl.ValueKind == JsonValueKind.String)
                {
                    var sku = skuEl.GetString() ?? "UNKNOWN";
                    bySku[sku] = (bySku.TryGetValue(sku, out var current) ? current : 0m) + a.EstimatedLoss;
                }
            }
            catch
            {
            }
        }

        var bySkuTop = bySku
            .Select(kv => new { sku = kv.Key, estimatedLoss = kv.Value })
            .OrderByDescending(x => x.estimatedLoss)
            .Take(10)
            .ToList();

        return Ok(new
        {
            from,
            to,
            lossEstimated = loss,
            alertsOpen = open,
            alertsResolved = resolved,
            falsePositives = 0,
            bySeverity,
            topSessions,
            bySkuTop
        });
    }
}