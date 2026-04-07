using System.Diagnostics;
using System.Drawing;
using Silk.NET.GLFW;

namespace Triangles.BaseApp;

public class Application(AppConfig config)
{
    private readonly Glfw _glfwApi = GlfwProvider.GLFW.Value;

    public readonly Window MainWindow = new(config.Name);

    public event Action<float>? Renderer;

    public void Run()
    {
        bool isMinimized = false;
        
        Stopwatch Stopwatch = Stopwatch.StartNew();
        TimeSpan _lastFrameTime = TimeSpan.Zero;
        
        MainWindow.Show();
        MainWindow.SizeChange += (window, size) => isMinimized = size.IsEmpty;

        while (MainWindow.IsOpen)
        {
            var now = Stopwatch.Elapsed;
            var delta = now - _lastFrameTime;
            _lastFrameTime = now;
            
            if (!isMinimized)
            {

                
                Renderer?.Invoke((float)delta.TotalSeconds);
            }
            
            _glfwApi.PollEvents();
        }
        
        _glfwApi.Terminate();
    }
}