using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/audit")]
public class AuditController : ControllerBase
{
    private readonly IReconciliationService _reconciliationService;

    public AuditController(IReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    [HttpPost("reconcile/{sessionId:guid}")]
    public async Task<ActionResult<ReconciliationResult>> Reconcile(Guid sessionId, CancellationToken ct)
    {
        var result = await _reconciliationService.RunAsync(sessionId, ct);
        return Ok(result);
    }
}