using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/areas")]
public class AdminAreasController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminAreasController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<AreaAdminResponse>> List(CancellationToken ct) => _service.ListAreasAsync(ct);

    [HttpGet("{areaId:guid}")]
    public async Task<IActionResult> Get(Guid areaId, CancellationToken ct)
    {
        var item = await _service.GetAreaAsync(areaId, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public Task<AreaAdminResponse> Create([FromBody] CreateAreaAdminRequest req, CancellationToken ct)
        => _service.CreateAreaAsync(req, ct);

    [HttpPut("{areaId:guid}")]
    public Task<AreaAdminResponse> Update(Guid areaId, [FromBody] UpdateAreaAdminRequest req, CancellationToken ct)
        => _service.UpdateAreaAsync(areaId, req, ct);

    [HttpDelete("{areaId:guid}")]
    public Task<bool> Delete(Guid areaId, CancellationToken ct)
        => _service.DeleteAreaAsync(areaId, ct);
}