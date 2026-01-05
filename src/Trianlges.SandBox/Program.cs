using Trianlges;

namespace Trianlges.SandBox;

public static class Program
{
    public static void Main(string[] args)
    {
        using var window = new Window(800, 600, "Sandbox 0.0.1.1");

        var app = new GameApp(window, args);
        app.Run();
    }
}