using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Infrastructure.Data;

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
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<EventRecord> Events => Set<EventRecord>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<PlanogramAreaProduct> PlanogramAreaProducts => Set<PlanogramAreaProduct>();
    public DbSet<DeviceApiKey> DeviceApiKeys => Set<DeviceApiKey>();
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    public DbSet<FraudAnalysis> FraudAnalyses => Set<FraudAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasKey(x => x.ProductId);
        modelBuilder.Entity<ProductPrice>().HasKey(x => x.ProductPriceId);
        modelBuilder.Entity<Session>().HasKey(x => x.SessionId);
        modelBuilder.Entity<CartItem>().HasKey(x => x.CartItemId);
        modelBuilder.Entity<Payment>().HasKey(x => x.PaymentId);
        modelBuilder.Entity<Alert>().HasKey(x => x.AlertId);
        modelBuilder.Entity<EventRecord>().HasKey(x => x.EventId);
        modelBuilder.Entity<Area>().HasKey(x => x.AreaId);
        modelBuilder.Entity<DeviceApiKey>().HasKey(x => x.DeviceApiKeyId);
        modelBuilder.Entity<Resident>().HasKey(x => x.ResidentId);
        modelBuilder.Entity<IdempotencyKey>().HasKey(x => x.IdempotencyKeyId);
        modelBuilder.Entity<FraudAnalysis>().HasKey(x => x.FraudAnalysisId);

        modelBuilder.Entity<PlanogramAreaProduct>()
            .HasKey(x => new { x.AreaId, x.ProductId });

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Sku)
            .IsUnique();

        modelBuilder.Entity<DeviceApiKey>()
            .HasIndex(x => x.DeviceId);

        modelBuilder.Entity<Session>()
            .HasIndex(x => new { x.ResidentId, x.Status });

        modelBuilder.Entity<Session>()
            .HasIndex(x => new { x.DeviceId, x.Status });
    }
}