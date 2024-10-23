using Microsoft.AspNetCore.Mvc;
using goida.Models;

namespace goida.Controllers
{
    public class GitStatsController : Controller
    {
        private readonly GitHubService _gitHubService;

        public GitStatsController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Получаем статистику репозитория
                var stats = await _gitHubService.GetRepositoryStats("M3th4d0n", "goida");

                // Передаем данные в представление
                ViewData["CommitCount"] = stats.CommitCount;
                ViewData["LineCount"] = stats.LineCount;
                ViewData["Commits"] = stats.Commits; // Передаем список коммитов

                return View();
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок, например, если репозиторий не найден
                ViewData["ErrorMessage"] = "Error fetching repository stats: " + ex.Message;
                return View("Error"); // Можно создать отдельное представление для ошибок
            }
        }

    }
}