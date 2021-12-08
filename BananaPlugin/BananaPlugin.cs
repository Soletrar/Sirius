using Newtonsoft.Json;
using Sirius.Core;

namespace BananaPlugin;

internal class BananaPlugin : ISiriusPlugin
{
    public void Start()
    {
        ThreadPool.QueueUserWorkItem(async _ =>
        {
            await Task.Delay(5000);
            await WriteLogTask();
        });

        Emulator.Walk(22, 57);
    }

    private static async Task WriteLogTask()
    {
        var json = await new HttpClient().GetStringAsync("https://jsonplaceholder.typicode.com/todos/1");
        var todo = JsonConvert.DeserializeObject<TodoModel>(json);

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        await File.WriteAllTextAsync(Path.Combine(desktopPath, "banana.txt"),
            $"[{DateTime.Now.ToLongTimeString()}] Todo ({todo?.Id}) -> {todo?.Title}");
    }
}

public class TodoModel
{
    [JsonProperty("completed")] public bool Completed;

    [JsonProperty("id")] public int Id;

    [JsonProperty("title")] public string? Title;
    [JsonProperty("userId")] public int    UserId;
}