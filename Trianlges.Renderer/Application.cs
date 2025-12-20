using GLFW;

namespace Trianlges.Renderer;

public class Application
{
    private readonly Window _window;
    private string[] _args;

    public Application(Window window, string[] args)
    {
        _window = window;
        _args = args;
    }
    
    public void Run()
    {
        Initializer();
        
        while (!Glfw.WindowShouldClose(_window))
        {

            
            Glfw.PollEvents();
        }
    }

    private void Initializer()
    {
        
    }
}