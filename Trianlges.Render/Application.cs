namespace Trianlges.Render;

public abstract class Application
{
    protected readonly Window _window;
    protected string[] _args;

    public Application(Window window, string[] args)
    {
        _window = window;
        _args = args;
    }
    
    public void Run()
    {
        Initializer();
        
        while (!_window.IsClose)
        {
            Render();
            
            _window.DispatchMessage();
        }
    }

    protected abstract void Initializer();
    protected abstract void Render();
}