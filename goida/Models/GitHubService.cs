using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using Newtonsoft.Json;
using System.Text.Json;

namespace goida.Models
{
    public class GitHubService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        public GitHubService(HttpClient client, IMemoryCache memoryCache)
        {
            _client = client;
            _cache = memoryCache;
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
        }

        // Метод для получения списка коммитов
        private async Task<List<Commit>> GetCommits(string owner, string repo)
        {
            var response = await _client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/commits");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var commitsArray = JArray.Parse(content);
            var commits = new List<Commit>();

            foreach (var item in commitsArray)
            {
                var commit = new Commit
                {
                    Sha = item["sha"].ToString(),
                    Message = item["commit"]["message"].ToString(),
                    Author = item["commit"]["author"]["name"].ToString(),
                    Date = DateTime.Parse(item["commit"]["author"]["date"].ToString())
                };
                commits.Add(commit);
                Debug.WriteLine(commit);
            }

            Debug.WriteLine(commits);
            return commits; // Возвращаем список коммитов
        }


        public class Commit
        {
            public string Sha { get; set; }
            public string Message { get; set; }
            public string Author { get; set; }
            public DateTime Date { get; set; }
        }

        // Обновленный метод для получения статистики репозитория
        public async Task<(int CommitCount, int LineCount, List<Commit> Commits)> GetRepositoryStats(string owner, string repo)
        {
            // Проверяем, есть ли данные в кэше
            if (_cache.TryGetValue((owner, repo), out (int CommitCount, int LineCount, List<Commit> Commits) cachedStats))
            {
                return cachedStats;
            }

            // Если данных нет, делаем запрос
            int commitCount = await GetCommitCount(owner, repo);
            List<Commit> commits = await GetCommits(owner, repo); // Получаем список коммитов
            int lineCount = 1000; // Здесь можно добавить логику для получения реального количества строк кода

            var stats = (commitCount, lineCount, commits);

            // Кэшируем данные на 30 минут (или любое другое время)
            _cache.Set((owner, repo), stats, TimeSpan.FromMinutes(1));

            return stats;
        }



        private async Task<int> GetCommitCount(string owner, string repo)
        {
            var response = await _client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/commits");
            if (response.StatusCode == HttpStatusCode.Forbidden && 
                response.Headers.Contains("X-RateLimit-Remaining") &&
                int.TryParse(response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault(), out int remaining) &&
                remaining == 0)
            {
                // Логика при достижении лимита запросов
                throw new HttpRequestException("API rate limit exceeded. Please try again later.");
            }

            response.EnsureSuccessStatusCode();

            // Получаем контент ответа
            var content = await response.Content.ReadAsStringAsync();
            var commits = JArray.Parse(content);
            return commits.Count;
        }
    }
}
