using MiniMercadoInteligente.Domain.Events;
using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Projections;

public class SmartCartProjectionService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductCatalogRepository _productCatalogRepository;

    public SmartCartProjectionService(
        ICartRepository cartRepository,
        IProductCatalogRepository productCatalogRepository)
    {
        _cartRepository = cartRepository;
        _productCatalogRepository = productCatalogRepository;
    }

    public async Task ApplyAsync(ProductPicked evt, CancellationToken ct = default)
    {
        var product = await _productCatalogRepository.GetByIdAsync(evt.ProductId, ct);
        if (product is null) return;

        await _cartRepository.AddOrIncrementItemAsync(
            evt.SessionId,
            evt.ProductId,
            (int)evt.Quantity,
            product.CurrentPrice,
            ct);
    }

    public async Task ApplyAsync(ProductReturned evt, CancellationToken ct = default)
    {
        await _cartRepository.DecrementOrRemoveItemAsync(
            evt.SessionId,
            evt.ProductId,
            (int)evt.Quantity,
            ct);
    }
}