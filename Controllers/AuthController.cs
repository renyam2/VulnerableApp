using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using BCrypt.Net;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Login()
        {
            _logger.LogInformation("Inicio Auth.Login (GET). IP:{IP}",
                HttpContext.Connection.RemoteIpAddress);
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // NUNCA se registra 'password' en ningún log, solo el username.
            _logger.LogInformation("Inicio Auth.Login (POST). Usuario:{User} IP:{IP}", username, ip);

            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Evento de autenticación: login fallido. Usuario:{User} IP:{IP}",
                        username, ip);
                    ViewBag.Error = "Credenciales inválidas";
                    stopwatch.Stop();
                    return View();
                }

                HttpContext.Session.SetString("User", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                _logger.LogInformation("Evento de autenticación: login exitoso. Usuario:{User} IP:{IP}",
                    user.Username, ip);

                stopwatch.Stop();
                _logger.LogInformation("Fin Auth.Login. Duración:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Auth.Login. Usuario:{User} IP:{IP} Duración:{ElapsedMs}ms",
                    username, ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Dashboard()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userId = HttpContext.Session.GetInt32("UserId");

            _logger.LogInformation("Inicio Auth.Dashboard. UserId:{UserId} IP:{IP}", userId, ip);

            try
            {
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Acceso no autenticado a Auth.Dashboard. IP:{IP}", ip);
                    return RedirectToAction("Login");
                }

                var user = _db.Users.Find(userId.Value);

                stopwatch.Stop();
                _logger.LogInformation("Fin Auth.Dashboard. Usuario:{User} Duración:{ElapsedMs}ms",
                    user?.Username, stopwatch.ElapsedMilliseconds);

                return View(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Auth.Dashboard. UserId:{UserId} IP:{IP} Duración:{ElapsedMs}ms",
                    userId, ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Logout()
        {
            var user = HttpContext.Session.GetString("User");
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Evento de autenticación: logout. Usuario:{User} IP:{IP}", user, ip);

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
