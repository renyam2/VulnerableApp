using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger;

        public ApiController(AppDbContext db, ILogger<ApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            _logger.LogInformation("Inicio Api.GetUser. RequestedId:{Id} CurrentUserId:{CurrentUserId} IP:{IP}",
                id, currentUserId, ip);

            try
            {
                if (!currentUserId.HasValue)
                {
                    _logger.LogWarning("Api.GetUser: acceso no autenticado. IP:{IP}", ip);
                    stopwatch.Stop();
                    return Unauthorized();
                }

                if (id != currentUserId.Value)
                {
                    _logger.LogWarning("Api.GetUser: intento de acceso a recurso ajeno. CurrentUserId:{CurrentUserId} RequestedId:{Id} IP:{IP}",
                        currentUserId, id, ip);
                    stopwatch.Stop();
                    return Forbid();
                }

                var user = _db.Users.Find(id);
                if (user == null)
                {
                    _logger.LogWarning("Api.GetUser: usuario no encontrado. Id:{Id} IP:{IP}", id, ip);
                    stopwatch.Stop();
                    return NotFound();
                }

                stopwatch.Stop();
                _logger.LogInformation("Fin Api.GetUser. Duración:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Api.GetUser. Id:{Id} IP:{IP} Duración:{ElapsedMs}ms",
                    id, ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Inicio Api.GetAllUsers. IP:{IP}", ip);

            try
            {
                var users = _db.Users.ToList();

                stopwatch.Stop();
                _logger.LogInformation("Fin Api.GetAllUsers. Total:{Count} Duración:{ElapsedMs}ms",
                    users.Count, stopwatch.ElapsedMilliseconds);

                return Ok(users);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Api.GetAllUsers. IP:{IP} Duración:{ElapsedMs}ms",
                    ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

