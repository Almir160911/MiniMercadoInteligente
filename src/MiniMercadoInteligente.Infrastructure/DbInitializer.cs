using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Infrastructure;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        if (!await db.Products.AnyAsync())
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Sku = "789100000001",
                Name = "Água 500ml",
                Barcode = "789100000001",
                QrCode = "QR-789100000001",
                NominalWeightGrams = 520,
                WeightToleranceGrams = 30,
                IsWeightControlled = true,
                Active = true
            };

            db.Products.Add(product);

            db.ProductPrices.Add(new ProductPrice
            {
                ProductPriceId = Guid.NewGuid(),
                ProductId = product.ProductId,
                Price = 3.50m,
                Currency = "BRL",
                EffectiveFrom = DateTime.UtcNow,
                Active = true
            });

            await db.SaveChangesAsync();
        }
    }
}