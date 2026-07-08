using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var stopwatch = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("Usuario") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                "Inicio Search.Index. Usuario:{User} IP:{IP} Path:{Path} Search:{Search}",
                user, ip, HttpContext.Request.Path, search);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    _logger.LogWarning("Search.Index sin término de búsqueda. Usuario:{User} IP:{IP}", user, ip);
                    stopwatch.Stop();
                    return View(new List<User>());
                }

                var users = _db.Users
                    .Where(u => u.Username.Contains(search))
                    .ToList();

                if (users.Count == 0)
                {
                    _logger.LogWarning("Search.Index sin resultados. Usuario:{User} IP:{IP} Search:{Search}",
                        user, ip, search);
                }

                stopwatch.Stop();
                _logger.LogInformation("Fin Search.Index. Resultados:{Count} Duración:{ElapsedMs}ms",
                    users.Count, stopwatch.ElapsedMilliseconds);

                return View(users);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Search.Index. Usuario:{User} IP:{IP} Search:{Search} Duración:{ElapsedMs}ms",
                    user, ip, search, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
