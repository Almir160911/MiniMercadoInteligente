using MiniMercadoInteligente.Domain.Ports;

namespace MiniMercadoInteligente.Application.Services;

public class ProductAreaMismatchFraudRule : IFraudRule
{
    private readonly IProductCatalogRepository _products;

    public ProductAreaMismatchFraudRule(IProductCatalogRepository products)
    {
        _products = products;
    }

    public string Name => "ProductAreaMismatch";

    public async Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var flags = new List<FraudFlag>();

        foreach (var cartItem in context.CartItems)
        {
            var product = await _products.GetByIdAsync(cartItem.ProductId, ct);
            if (product is null) continue;

            var relatedEvents = context.Events
                .Where(e => e.ProductId == cartItem.ProductId || 
                            (!string.IsNullOrWhiteSpace(cartItem.Sku) && e.PayloadJson.Contains(cartItem.Sku)))
                .ToList();

            foreach (var evt in relatedEvents)
            {
                var sourceArea = evt.Source;
                if (string.IsNullOrWhiteSpace(sourceArea)) continue;
                if (string.IsNullOrWhiteSpace(product.AreaCode)) continue;

                if (!string.Equals(product.AreaCode, sourceArea, StringComparison.OrdinalIgnoreCase))
                {
                    flags.Add(new FraudFlag(
                        "PRODUCT_AREA_MISMATCH",
                        $"Produto {product.Name} detectado em área diferente da esperada.",
                        20,
                        "MEDIUM",
                        new
                        {
                            productId = product.ProductId,
                            expectedArea = product.AreaCode,
                            detectedArea = sourceArea
                        }));
                }
            }
        }

        return flags;
    }
}