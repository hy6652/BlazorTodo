using System.Diagnostics;

namespace BlazorTodo.Server.Services
{
    public class TestTaskService
    {
        public TestTaskService() { }

        public async Task SumPageSizeAsync()
        {
            HttpClient client = new()
            {
                MaxResponseContentBufferSize = 1_000_000
            };

            IEnumerable<string> s_urlList = new string[]
            {
                "https://learn.microsoft.com",
                "https://learn.microsoft.com/aspnet/core",
                "https://learn.microsoft.com/azure",
                "https://learn.microsoft.com/azure/devops",
                "https://learn.microsoft.com/dotnet",
                "https://learn.microsoft.com/dynamics365",
                "https://learn.microsoft.com/education",
                "https://learn.microsoft.com/enterprise-mobility-security",
                "https://learn.microsoft.com/gaming",
                "https://learn.microsoft.com/graph",
                "https://learn.microsoft.com/microsoft-365",
                "https://learn.microsoft.com/office",
                "https://learn.microsoft.com/powershell",
                "https://learn.microsoft.com/sql",
                "https://learn.microsoft.com/surface",
                "https://learn.microsoft.com/system-center",
                "https://learn.microsoft.com/visualstudio",
                "https://learn.microsoft.com/windows",
                "https://learn.microsoft.com/xamarin"
            };

            var stopwatch = Stopwatch.StartNew();

            IEnumerable<Task<int>> dowloadTasksQuery = s_urlList.Select(x => ProcessUrlAsync(x, client));

            List<Task<int>> downloadTasks = dowloadTasksQuery.ToList();

            int total = 0;
            while (downloadTasks.Any())
            {
                Task<int> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);
                total += await finishedTask;
            }

            stopwatch.Stop();

            Debug.WriteLine($"\nTotal bytes returned:    {total:#,#}");
            Debug.WriteLine($"Elapsed time:              {stopwatch.Elapsed}\n");
        }

        public async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            var content = await client.GetByteArrayAsync(url);
            Debug.WriteLine($"{url,-60} {content.Length,10:#,#}");

            return content.Length;
        }
    }
}
