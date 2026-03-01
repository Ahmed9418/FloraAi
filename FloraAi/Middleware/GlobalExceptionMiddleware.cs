using System.Net;
using System.Text.Json;

namespace FloraAI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    { _next = next; _logger = logger; }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ غير متوقع: {Message}", ex.Message);
            ctx.Response.ContentType = "application/json";
            var (code, msg) = ex switch
            {
                InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "فشل التحقق من الهوية."),
                ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "حدث خطأ غير متوقع.")
            };
            ctx.Response.StatusCode = (int)code;
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { خطأ = msg, الحالة = (int)code }));
        }
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}