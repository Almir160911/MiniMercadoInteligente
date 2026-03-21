# MiniMercadoInteligente – Refatorado + PostgreSQL
## Subir Postgres
```bash
docker compose up -d
```
## Rodar API
```bash
dotnet restore
dotnet run --project src/MiniMercadoInteligente
```
## Criar Migrations
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p src/MiniMercadoInteligente.Infrastructure -s src/MiniMercadoInteligente
dotnet ef database update -p src/MiniMercadoInteligente.Infrastructure -s src/MiniMercadoInteligente
```

## v4.6 – Controllers conforme Especificação v4.2
- /api/v1, JWT + API Key, Idempotency-Key, X-Correlation-Id, ProblemDetails.

## v4.7 – Produto com QRCode e Peso
Inclui campos de peso/QRCode em Product, payload jsonb em Alert e endpoint GET /api/v1/products.

## v4.8 – Reconciliação por Peso com Tolerância Bidirecional
- Implementada regra: peso medido deve estar entre (nominal±tolerância) * quantidade.
- Eventos PRODUCT_ESTIMATED_FROM_WEIGHT devem enviar payload: { weightDeltaGrams, productId|sku|qrcode, confidence? }.
- Divergências e evidências detalhadas gravadas em alerts.payload_json (jsonb).

## v4.8.2 – CRUD de Produtos (Admin)
- Admin-only: GET/POST/PUT/DELETE /api/v1/admin/products (+ GET by id)
- DTOs: CreateProductRequest, UpdateProductRequest
# MiniMercadoInteligente
# MiniMercadoInteligente
