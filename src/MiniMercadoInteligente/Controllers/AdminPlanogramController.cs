using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/planogram")]
public class AdminPlanogramController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminPlanogramController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<PlanogramAreaProductAdminResponse>> List(CancellationToken ct)
        => _service.ListPlanogramAsync(ct);

    [HttpPost]
    public Task<PlanogramAreaProductAdminResponse> Upsert([FromBody] UpsertPlanogramAreaProductAdminRequest req, CancellationToken ct)
        => _service.UpsertPlanogramAsync(req, ct);

    [HttpDelete]
    public Task<bool> Delete([FromQuery] Guid areaId, [FromQuery] Guid productId, CancellationToken ct)
        => _service.DeletePlanogramAsync(areaId, productId, ct);
}