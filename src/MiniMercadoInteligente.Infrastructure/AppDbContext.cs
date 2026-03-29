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
    public DbSet<EventRecord> EventRecords => Set<EventRecord>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<DeviceApiKey> DeviceApiKeys => Set<DeviceApiKey>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<PlanogramAreaProduct> PlanogramAreaProducts => Set<PlanogramAreaProduct>();

    // Novas entidades para nível Amazon Go
    public DbSet<CameraDevice> CameraDevices => Set<CameraDevice>();
    public DbSet<WeightSensor> WeightSensors => Set<WeightSensor>();
    public DbSet<Gate> Gates => Set<Gate>();
    public DbSet<TrackingSnapshot> TrackingSnapshots => Set<TrackingSnapshot>();
    public DbSet<ShelfInteraction> ShelfInteractions => Set<ShelfInteraction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProduct(modelBuilder);
        ConfigureProductPrice(modelBuilder);
        ConfigureSession(modelBuilder);
        ConfigureCartItem(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigureEventRecord(modelBuilder);
        ConfigureAlert(modelBuilder);
        ConfigureDeviceApiKey(modelBuilder);
        ConfigureIdempotencyKey(modelBuilder);
        ConfigureArea(modelBuilder);
        ConfigurePlanogramAreaProduct(modelBuilder);

        // Novas tabelas Amazon Go
        ConfigureCameraDevice(modelBuilder);
        ConfigureWeightSensor(modelBuilder);
        ConfigureGate(modelBuilder);
        ConfigureTrackingSnapshot(modelBuilder);
        ConfigureShelfInteraction(modelBuilder);
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(x => x.ProductId);

            entity.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Barcode)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.QrCode)
                .HasMaxLength(200);

            entity.Property(x => x.CurrentPrice)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.UnitWeightGrams)
                .HasColumnType("numeric(18,3)");

            entity.HasIndex(x => x.Sku).IsUnique();
            entity.HasIndex(x => x.Barcode);
            entity.HasIndex(x => x.Active);
            entity.HasIndex(x => x.IsWeightControlled);
        });
    }

    private static void ConfigureProductPrice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.ToTable("product_prices");

            entity.HasKey(x => x.ProductPriceId);

            entity.HasIndex(x => x.ProductId);
        });
    }

    private static void ConfigureSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("sessions");

            entity.HasKey(x => x.SessionId);

            entity.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.FraudFlagsJson)
                .HasColumnType("jsonb");

            entity.Property(x => x.CurrentAreaCode)
                .HasMaxLength(100);

            entity.Property(x => x.ActiveTrackId)
                .HasMaxLength(100);

            entity.HasIndex(x => x.ResidentId);
            entity.HasIndex(x => x.CondominiumId);
            entity.HasIndex(x => x.DeviceId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartedAt);
            entity.HasIndex(x => x.EnteredAtUtc);
            entity.HasIndex(x => x.ActiveTrackId);
        });
    }

    private static void ConfigureCartItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");

            entity.HasKey(x => x.CartItemId);

            entity.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Source)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => new { x.SessionId, x.ProductId });
            entity.HasIndex(x => x.OccurredAtUtc);
        });
    }

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasKey(x => x.PaymentId);

            entity.Property(x => x.Method)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Amount)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.GatewayRef)
                .HasMaxLength(200);

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.GatewayRef);
            entity.HasIndex(x => x.CreatedAt);
        });
    }

    private static void ConfigureEventRecord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventRecord>(entity =>
        {
            entity.ToTable("event_records");

            entity.HasKey(x => x.EventId);

            entity.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Source)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PayloadJson)
                .HasColumnType("jsonb");

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.AreaId);
            entity.HasIndex(x => x.EventType);
            entity.HasIndex(x => x.OccurredAt);
        });
    }

    private static void ConfigureAlert(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts");

            entity.HasKey(x => x.AlertId);

            entity.Property(x => x.PayloadJson)
                .HasColumnType("jsonb");

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.CreatedAt);
        });
    }

    private static void ConfigureDeviceApiKey(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceApiKey>(entity =>
        {
            entity.ToTable("device_api_keys");

            entity.HasKey(x => x.DeviceApiKeyId);

            entity.HasIndex(x => x.DeviceId);
            entity.HasIndex(x => new { x.DeviceId, x.Active });
        });
    }

    private static void ConfigureIdempotencyKey(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdempotencyKey>(entity =>
        {
            entity.ToTable("idempotency_keys");

            entity.HasKey(x => x.IdempotencyKeyId);
        });
    }

    private static void ConfigureArea(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("areas");

            entity.HasKey(x => x.AreaId);
        });
    }

    private static void ConfigurePlanogramAreaProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlanogramAreaProduct>(entity =>
        {
            entity.ToTable("planogram_area_products");

            entity.HasKey(x => new { x.AreaId, x.ProductId });

            entity.HasIndex(x => x.ProductId);
        });
    }

    private static void ConfigureCameraDevice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CameraDevice>(entity =>
        {
            entity.ToTable("camera_devices");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CameraId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.AreaCode)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.RtspUrl)
                .HasMaxLength(500);

            entity.HasIndex(x => x.CameraId).IsUnique();
            entity.HasIndex(x => x.CondominiumId);
            entity.HasIndex(x => x.AreaId);
            entity.HasIndex(x => x.AreaCode);
            entity.HasIndex(x => x.Active);
        });
    }

    private static void ConfigureWeightSensor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeightSensor>(entity =>
        {
            entity.ToTable("weight_sensors");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SensorId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.AreaCode)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.CurrentWeightGrams)
                .HasColumnType("numeric(18,3)");

            entity.Property(x => x.LastWeightGrams)
                .HasColumnType("numeric(18,3)");

            entity.Property(x => x.ToleranceGrams)
                .HasColumnType("numeric(18,3)");

            entity.HasIndex(x => x.SensorId).IsUnique();
            entity.HasIndex(x => x.CondominiumId);
            entity.HasIndex(x => x.AreaId);
            entity.HasIndex(x => x.AreaCode);
            entity.HasIndex(x => x.Active);
        });
    }

    private static void ConfigureGate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gate>(entity =>
        {
            entity.ToTable("gates");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.GateId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(x => x.GateId).IsUnique();
            entity.HasIndex(x => x.CondominiumId);
            entity.HasIndex(x => x.Active);
        });
    }

    private static void ConfigureTrackingSnapshot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackingSnapshot>(entity =>
        {
            entity.ToTable("tracking_snapshots");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CameraId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.TrackId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.AreaCode)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Confidence)
                .HasColumnType("numeric(5,4)");

            entity.OwnsOne(x => x.BoundingBox, box =>
            {
                box.Property(b => b.X).HasColumnName("bbox_x").HasColumnType("numeric(18,3)");
                box.Property(b => b.Y).HasColumnName("bbox_y").HasColumnType("numeric(18,3)");
                box.Property(b => b.Width).HasColumnName("bbox_width").HasColumnType("numeric(18,3)");
                box.Property(b => b.Height).HasColumnName("bbox_height").HasColumnType("numeric(18,3)");
            });

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.CameraId);
            entity.HasIndex(x => x.TrackId);
            entity.HasIndex(x => x.AreaId);
            entity.HasIndex(x => x.AreaCode);
            entity.HasIndex(x => x.CapturedAtUtc);
        });
    }

    private static void ConfigureShelfInteraction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShelfInteraction>(entity =>
        {
            entity.ToTable("shelf_interactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.AreaCode)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.CameraId)
                .HasMaxLength(100);

            entity.Property(x => x.SensorId)
                .HasMaxLength(100);

            entity.Property(x => x.TrackId)
                .HasMaxLength(100);

            entity.Property(x => x.InteractionType)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.WeightDeltaGrams)
                .HasColumnType("numeric(18,3)");

            entity.Property(x => x.Confidence)
                .HasColumnType("numeric(5,4)");

            entity.Property(x => x.EvidenceId)
                .HasMaxLength(100);

            entity.Property(x => x.PayloadJson)
                .HasColumnType("jsonb");

            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.AreaId);
            entity.HasIndex(x => x.AreaCode);
            entity.HasIndex(x => x.TrackId);
            entity.HasIndex(x => x.SensorId);
            entity.HasIndex(x => x.CameraId);
            entity.HasIndex(x => x.InteractionType);
            entity.HasIndex(x => x.OccurredAtUtc);
            entity.HasIndex(x => x.Reconciled);
        });
    }
}