/* 
using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminPanelService _service;

    public AdminProductsController(IAdminPanelService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<ProductAdminResponse>> Get([FromQuery] bool? active, CancellationToken ct)
        => _service.ListProductsAsync(active, ct);

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> Get(Guid productId, CancellationToken ct)
    {
        var item = await _service.GetProductAsync(productId, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public Task<ProductAdminResponse> Create([FromBody] CreateProductAdminRequest req, CancellationToken ct)
        => _service.CreateProductAsync(req, ct);

    [HttpPut("{productId:guid}")]
    public Task<ProductAdminResponse> Update(Guid productId, [FromBody] UpdateProductAdminRequest req, CancellationToken ct)
        => _service.UpdateProductAsync(productId, req, ct);

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken ct)
    {
        var ok = await _service.DeleteProductAsync(productId, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{productId:guid}/price")]
    public Task<ProductPriceAdminResponse> SetPrice(Guid productId, [FromBody] SetProductPriceAdminRequest req, CancellationToken ct)
        => _service.SetProductPriceAsync(productId, req, ct);
}
*/