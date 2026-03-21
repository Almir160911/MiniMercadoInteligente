using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 
using MiniMercadoInteligente.Domain; 
using MiniMercadoInteligente.Infrastructure.Security;

namespace MiniMercadoInteligente;
public static class DbInitializer{
 public static async Task SeedAsync(IServiceProvider sp,IConfiguration cfg){
  if(!cfg.GetValue<bool>("Seeding:Enabled")) return;
  using var scope=sp.CreateScope(); var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await db.Database.MigrateAsync();
  if(!await db.Products.AnyAsync()){
   var p1=new Product{ProductId=Guid.NewGuid(),Sku="789100000001",Barcode="789100000001",QrCode="QR-789100000001",Name="Água 500ml",NominalWeightGrams=520,WeightToleranceGrams=30,IsWeightControlled=true,Active=true};
   db.Products.Add(p1); db.ProductPrices.Add(new ProductPrice{ProductPriceId=Guid.NewGuid(),ProductId=p1.ProductId,Price=3.50m,Currency="BRL",Active=true});
  }
  if(!await db.DeviceApiKeys.AnyAsync()){
   var plain=cfg.GetSection("ApiKeys:Devices").GetChildren().FirstOrDefault()?.GetValue<string>("apiKey") ?? "CHANGE_ME_SENSOR01";
   db.DeviceApiKeys.Add(new DeviceApiKey{DeviceApiKeyId=Guid.NewGuid(),DeviceId="SENSOR-01",ApiKeyHash=Hashing.Sha256(plain),DeviceType="Sensor",Active=true});
  }
  await db.SaveChangesAsync();
 }
}
