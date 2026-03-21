using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<EventRecord> EventRecords => Set<EventRecord>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<DeviceApiKey> DeviceApiKeys => Set<DeviceApiKey>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<PlanogramAreaProduct> PlanogramAreaProducts => Set<PlanogramAreaProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventRecord>()
            .Property(x => x.PayloadJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Alert>()
            .Property(x => x.PayloadJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PlanogramAreaProduct>()
            .HasKey(x => new { x.AreaId, x.ProductId });
    }
}