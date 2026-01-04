namespace Trianlges;

public abstract class Application
{
    private readonly string[] _args;
    protected readonly Window MainWindow;

    protected Application(Window mainWindow, string[] args)
    {
        MainWindow = mainWindow;
        _args = args;
    }

    public void Run()
    {
        Initializer(_args);

        while (!MainWindow.IsClose)
        {
            Render();

            MainWindow.DispatchMessage();
        }

        OnExit();
    }

    protected abstract void Initializer(string[] args);

    protected virtual void Render()
    {
    }

    protected virtual void OnExit()
    {
    }
}