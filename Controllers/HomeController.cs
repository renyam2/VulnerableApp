using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var stopwatch = Stopwatch.StartNew();
        var user = User.Identity?.Name ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        _logger.LogInformation("Inicio Home.Index. Usuario:{User} IP:{IP}", user, ip);

        try
        {
            var result = View();

            stopwatch.Stop();
            _logger.LogInformation("Fin Home.Index. Duración:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error en Home.Index. Usuario:{User} IP:{IP} Duración:{ElapsedMs}ms",
                user, ip, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public IActionResult Privacy()
    {
        var stopwatch = Stopwatch.StartNew();
        var user = User.Identity?.Name ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        _logger.LogInformation("Inicio Home.Privacy. Usuario:{User} IP:{IP}", user, ip);

        try
        {
            var result = View();

            stopwatch.Stop();
            _logger.LogInformation("Fin Home.Privacy. Duración:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error en Home.Privacy. Usuario:{User} IP:{IP} Duración:{ElapsedMs}ms",
                user, ip, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var stopwatch = Stopwatch.StartNew();
        var user = User.Identity?.Name ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        _logger.LogWarning("Inicio Home.Error. Usuario:{User} IP:{IP} RequestId:{RequestId}",
            user, ip, requestId);

        var result = View(new ErrorViewModel { RequestId = requestId });

        stopwatch.Stop();
        _logger.LogInformation("Fin Home.Error. Duración:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

        return result;
    }
}
