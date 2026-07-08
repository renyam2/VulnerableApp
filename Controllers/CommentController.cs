using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        private static List<string> _comments = new();
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Inicio Comment.Index. IP:{IP}", ip);

            try
            {
                var result = View(_comments);

                stopwatch.Stop();
                _logger.LogInformation("Fin Comment.Index. Total:{Count} Duración:{ElapsedMs}ms",
                    _comments.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Comment.Index. IP:{IP} Duración:{ElapsedMs}ms",
                    ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Inicio Comment.AddComment. IP:{IP} LongitudComentario:{Length}",
                ip, comment?.Length ?? 0);

            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    _comments.Add(comment);
                }
                else
                {
                    _logger.LogWarning("Comment.AddComment rechazado: comentario vacío. IP:{IP}", ip);
                }

                stopwatch.Stop();
                _logger.LogInformation("Fin Comment.AddComment. Duración:{ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error en Comment.AddComment. IP:{IP} Duración:{ElapsedMs}ms",
                    ip, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
