using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMercadoInteligente.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EstimatedLoss = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.AlertId);
                });

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AreaCode = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SensorId = table.Column<string>(type: "text", nullable: false),
                    CameraId = table.Column<string>(type: "text", nullable: false),
                    ConfidenceBase = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "camera_devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CameraId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RtspUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsEntryCamera = table.Column<bool>(type: "boolean", nullable: false),
                    IsExitCamera = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    CartItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SensorId = table.Column<string>(type: "text", nullable: true),
                    CameraId = table.Column<string>(type: "text", nullable: true),
                    AreaCode = table.Column<string>(type: "text", nullable: true),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    EvidenceId = table.Column<string>(type: "text", nullable: true),
                    Confidence = table.Column<decimal>(type: "numeric", nullable: false),
                    IsReconciled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.CartItemId);
                });

            migrationBuilder.CreateTable(
                name: "device_api_keys",
                columns: table => new
                {
                    DeviceApiKeyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    ApiKeyHash = table.Column<string>(type: "text", nullable: false),
                    DeviceType = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_api_keys", x => x.DeviceApiKeyId);
                });

            migrationBuilder.CreateTable(
                name: "event_records",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_records", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "gates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GateId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEntryGate = table.Column<bool>(type: "boolean", nullable: false),
                    IsExitGate = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "idempotency_keys",
                columns: table => new
                {
                    IdempotencyKeyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_keys", x => x.IdempotencyKeyId);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TriggerSource = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAutoCheckout = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentMethodToken = table.Column<string>(type: "text", nullable: true),
                    GatewayRef = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GatewayPayloadJson = table.Column<string>(type: "text", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.PaymentId);
                });

            migrationBuilder.CreateTable(
                name: "planogram_area_products",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedPosition = table.Column<string>(type: "text", nullable: false),
                    WeightToleranceGrams = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planogram_area_products", x => new { x.AreaId, x.ProductId });
                });

            migrationBuilder.CreateTable(
                name: "product_prices",
                columns: table => new
                {
                    ProductPriceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_prices", x => x.ProductPriceId);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    QrCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AreaCode = table.Column<string>(type: "text", nullable: false),
                    NominalWeightGrams = table.Column<int>(type: "integer", nullable: true),
                    WeightToleranceGrams = table.Column<int>(type: "integer", nullable: false),
                    IsWeightControlled = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitWeightGrams = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    IsSoldByUnit = table.Column<bool>(type: "boolean", nullable: false),
                    VisionLabel = table.Column<string>(type: "text", nullable: true),
                    DetectionConfidenceThreshold = table.Column<decimal>(type: "numeric", nullable: false),
                    IsHighRisk = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnteredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntryMethod = table.Column<string>(type: "text", nullable: false),
                    EntryGateId = table.Column<string>(type: "text", nullable: true),
                    ExitGateId = table.Column<string>(type: "text", nullable: true),
                    CurrentAreaCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ActiveTrackId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TrackingLocked = table.Column<bool>(type: "boolean", nullable: false),
                    TrackingConfidence = table.Column<decimal>(type: "numeric", nullable: false),
                    AutoCheckoutTriggered = table.Column<bool>(type: "boolean", nullable: false),
                    AutoCheckoutAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FraudSuspected = table.Column<bool>(type: "boolean", nullable: false),
                    FraudScore = table.Column<int>(type: "integer", nullable: false),
                    FraudFlagsJson = table.Column<string>(type: "jsonb", nullable: false),
                    BlockReason = table.Column<string>(type: "text", nullable: true),
                    ReconciliationVersion = table.Column<string>(type: "text", nullable: true),
                    DefaultPaymentMethodSnapshot = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.SessionId);
                });

            migrationBuilder.CreateTable(
                name: "shelf_interactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CameraId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SensorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TrackId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InteractionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WeightDeltaGrams = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    EvidenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Reconciled = table.Column<bool>(type: "boolean", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shelf_interactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tracking_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CameraId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TrackId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bbox_x = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    bbox_y = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    bbox_width = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    bbox_height = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    CapturedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracking_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "weight_sensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentWeightGrams = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    LastWeightGrams = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    ToleranceGrams = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    LastReadingAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weight_sensors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_CreatedAt",
                table: "alerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_SessionId",
                table: "alerts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Status",
                table: "alerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Type",
                table: "alerts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_camera_devices_Active",
                table: "camera_devices",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_camera_devices_AreaCode",
                table: "camera_devices",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_camera_devices_AreaId",
                table: "camera_devices",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_devices_CameraId",
                table: "camera_devices",
                column: "CameraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_camera_devices_CondominiumId",
                table: "camera_devices",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_OccurredAtUtc",
                table: "cart_items",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_ProductId",
                table: "cart_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_SessionId",
                table: "cart_items",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_SessionId_ProductId",
                table: "cart_items",
                columns: new[] { "SessionId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_device_api_keys_DeviceId",
                table: "device_api_keys",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_device_api_keys_DeviceId_Active",
                table: "device_api_keys",
                columns: new[] { "DeviceId", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_event_records_AreaId",
                table: "event_records",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_event_records_EventType",
                table: "event_records",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_event_records_OccurredAt",
                table: "event_records",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_event_records_ProductId",
                table: "event_records",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_event_records_SessionId",
                table: "event_records",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_gates_Active",
                table: "gates",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_gates_CondominiumId",
                table: "gates",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_gates_GateId",
                table: "gates",
                column: "GateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_CreatedAt",
                table: "payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_payments_GatewayRef",
                table: "payments",
                column: "GatewayRef");

            migrationBuilder.CreateIndex(
                name: "IX_payments_SessionId",
                table: "payments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_Status",
                table: "payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_planogram_area_products_ProductId",
                table: "planogram_area_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_prices_ProductId",
                table: "product_prices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_products_Active",
                table: "products",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_products_Barcode",
                table: "products",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_products_IsWeightControlled",
                table: "products",
                column: "IsWeightControlled");

            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                table: "products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_ActiveTrackId",
                table: "sessions",
                column: "ActiveTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_CondominiumId",
                table: "sessions",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_DeviceId",
                table: "sessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_EnteredAtUtc",
                table: "sessions",
                column: "EnteredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_ResidentId",
                table: "sessions",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_StartedAt",
                table: "sessions",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_Status",
                table: "sessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_AreaCode",
                table: "shelf_interactions",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_AreaId",
                table: "shelf_interactions",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_CameraId",
                table: "shelf_interactions",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_InteractionType",
                table: "shelf_interactions",
                column: "InteractionType");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_OccurredAtUtc",
                table: "shelf_interactions",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_ProductId",
                table: "shelf_interactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_Reconciled",
                table: "shelf_interactions",
                column: "Reconciled");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_SensorId",
                table: "shelf_interactions",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_SessionId",
                table: "shelf_interactions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_shelf_interactions_TrackId",
                table: "shelf_interactions",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_AreaCode",
                table: "tracking_snapshots",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_AreaId",
                table: "tracking_snapshots",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_CameraId",
                table: "tracking_snapshots",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_CapturedAtUtc",
                table: "tracking_snapshots",
                column: "CapturedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_SessionId",
                table: "tracking_snapshots",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_snapshots_TrackId",
                table: "tracking_snapshots",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_weight_sensors_Active",
                table: "weight_sensors",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_weight_sensors_AreaCode",
                table: "weight_sensors",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_weight_sensors_AreaId",
                table: "weight_sensors",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_weight_sensors_CondominiumId",
                table: "weight_sensors",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_weight_sensors_SensorId",
                table: "weight_sensors",
                column: "SensorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "camera_devices");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "device_api_keys");

            migrationBuilder.DropTable(
                name: "event_records");

            migrationBuilder.DropTable(
                name: "gates");

            migrationBuilder.DropTable(
                name: "idempotency_keys");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "planogram_area_products");

            migrationBuilder.DropTable(
                name: "product_prices");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "shelf_interactions");

            migrationBuilder.DropTable(
                name: "tracking_snapshots");

            migrationBuilder.DropTable(
                name: "weight_sensors");
        }
    }
}
