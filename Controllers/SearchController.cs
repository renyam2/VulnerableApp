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
            _logger.LogInformation("Entrando a Search.Index");

            _logger.LogInformation(
                "Usuario:{User} IP:{IP} Path:{Path}",
                HttpContext.Session.GetString("Usuario"),
                HttpContext.Connection.RemoteIpAddress,
                HttpContext.Request.Path);

            if (string.IsNullOrEmpty(search))
                return View(new List<User>());

            var users = _db.Users
                .Where(u => u.Username.Contains(search))
                .ToList();

            return View(users);
        }
    }
}