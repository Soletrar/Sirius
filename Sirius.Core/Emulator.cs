namespace Sirius.Core;

public static class Emulator
{
    public static string? Name { get; set; }

    static Emulator()
    {
        ThreadPool.SetMaxThreads(16, 16);
    }

    public static void RunTask(Action action, int startDelay = 0)
    {
        ThreadPool.QueueUserWorkItem(async _ =>
        {
            if (startDelay > 0) await Task.Delay(startDelay);

            action();
        });
    }

    public static void Walk(int x, int y)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        File.WriteAllText(Path.Combine(desktopPath, "Emulator.txt"),
            $"[{DateTime.Now.ToLongTimeString()}] [{Name}] Walking to {x}:{y}");
    }
}