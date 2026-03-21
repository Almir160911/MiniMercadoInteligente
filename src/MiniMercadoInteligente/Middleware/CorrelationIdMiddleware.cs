namespace MiniMercadoInteligente.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var cid) || string.IsNullOrWhiteSpace(cid))
        {
            cid = Guid.NewGuid().ToString("N");
            context.Request.Headers[HeaderName] = cid;
        }

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = cid.ToString();
            return Task.CompletedTask;
        });

        context.Items[HeaderName] = cid.ToString();
        await _next(context);
    }
}
