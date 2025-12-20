using GLFW;

namespace Trianlges.SandBox;

public static class Program
{
    public static void Main(string[] args)
    {
        var window = Glfw.CreateWindow(1280, 720, "GLFW", default, default);
        
        while (!Glfw.WindowShouldClose(window))
        {

            
            Glfw.PollEvents();
        }
        
        Glfw.DestroyWindow(window);
    }
}