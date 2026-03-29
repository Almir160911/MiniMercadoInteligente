using Microsoft.EntityFrameworkCore;
using MiniMercadoInteligente.Infrastructure.Data;
using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Api.Middleware;

public class IdempotencyMiddleware
{
    private const string HeaderName = "Idempotency-Key";
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var key) || string.IsNullOrWhiteSpace(key))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.ToString().ToLowerInvariant();
        var method = context.Request.Method.ToUpperInvariant();
        var now = DateTime.UtcNow;

        var expired = await db.IdempotencyKeys.Where(x => x.ExpiresAt < now).ToListAsync(context.RequestAborted);
        if (expired.Count > 0)
        {
            db.IdempotencyKeys.RemoveRange(expired);
            await db.SaveChangesAsync(context.RequestAborted);
        }

        var existing = await db.IdempotencyKeys.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == key!.ToString() && x.Method == method && x.Path == path && x.ExpiresAt >= now, context.RequestAborted);

        if (existing is not null)
        {
            context.Response.StatusCode = existing.StatusCode;
            context.Response.ContentType = existing.ContentType;
            await context.Response.WriteAsync(existing.ResponseBody);
            return;
        }

        var originalBody = context.Response.Body;
        await using var mem = new MemoryStream();
        context.Response.Body = mem;

        await _next(context);

        mem.Position = 0;
        var bodyText = await new StreamReader(mem).ReadToEndAsync();

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            var rec = new IdempotencyKey
            {
                IdempotencyKeyId = Guid.NewGuid(),
                Key = key!.ToString(),
                Method = method,
                Path = path,
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType ?? "application/json",
                ResponseBody = bodyText,
                CreatedAt = now,
                ExpiresAt = now.AddHours(24)
            };

            db.IdempotencyKeys.Add(rec);
            try { await db.SaveChangesAsync(context.RequestAborted); } catch { }
        }

        mem.Position = 0;
        await mem.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
