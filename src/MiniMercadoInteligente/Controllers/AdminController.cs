using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Application.Contracts;
using MiniMercadoInteligente.Application.Services;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IProductCrudService _productService;
    private readonly IReconciliationService _reconciliationService;

    public AdminController(
        IAdminService adminService,
        IProductCrudService productService,
        IReconciliationService reconciliationService)
    {
        _adminService = adminService;
        _productService = productService;
        _reconciliationService = reconciliationService;
    }

    [HttpPost("device-api-key")]
    public async Task<IActionResult> UpsertDeviceApiKey(
        [FromBody] UpsertDeviceApiKeyRequest request,
        CancellationToken ct)
    {
        var result = await _adminService.UpsertDeviceApiKeyAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<ProductResponse>>> ListProducts(CancellationToken ct)
    {
        return Ok(await _productService.ListAsync(ct));
    }

    [HttpGet("products/{productId:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(Guid productId, CancellationToken ct)
    {
        var product = await _productService.GetAsync(productId, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost("products")]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var result = await _productService.CreateAsync(request, ct);
        return Ok(result);
    }

    [HttpPut("products/{productId:guid}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        Guid productId,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        var result = await _productService.UpdateAsync(productId, request, ct);
        return Ok(result);
    }

    [HttpDelete("products/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken ct)
    {
        var ok = await _productService.DeleteAsync(productId, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("sessions/{sessionId:guid}/reconcile")]
    public async Task<ActionResult<ReconciliationResult>> Reconcile(Guid sessionId, CancellationToken ct)
    {
        var result = await _reconciliationService.RunAsync(sessionId, ct);
        return Ok(result);
    }
}