namespace VulnerableApp.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Excepción no controlada. Path:{Path} Method:{Method} IP:{IP}",
                context.Request.Path, context.Request.Method,
                context.Connection.RemoteIpAddress);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Ha ocurrido un error interno.");
            }
        }
    }
}
