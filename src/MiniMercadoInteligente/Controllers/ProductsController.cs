using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMercadoInteligente.Domain.Entities;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductCatalogRepository _catalog;
    public ProductsController(IProductCatalogRepository catalog) => _catalog = catalog;

    [HttpGet]
    [Authorize(Policy = "ResidentOrKiosk")]
    public async Task<ActionResult<List<Product>>> List(CancellationToken ct)
    {
        var list = await _catalog.ListProductsAsync(ct);
        return Ok(list.Where(p => p.Active).ToList());
    }
}
