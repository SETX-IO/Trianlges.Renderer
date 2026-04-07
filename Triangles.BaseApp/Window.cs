using System.Drawing;
using Silk.NET.GLFW;

namespace Triangles.BaseApp;

public unsafe class Window
{
    private readonly Glfw _glfwApi;
    internal readonly WindowHandle* WindowPtr;
    
    public string Title { get; set; }

    public Size Size
    {
        get
        {
            _glfwApi.GetWindowSize(WindowPtr, out int width, out var height);
            return new Size(width, height);
        }
        set
        {
            if (!Size.Equals(value) && Size != Size.Empty)
                _glfwApi.SetWindowSize(WindowPtr, value.Width, value.Height);
        }
    }

    public bool IsOpen => !_glfwApi.WindowShouldClose(WindowPtr);
    public event Action<Window, Size>? SizeChange;

    public Window(string title)
    {
        _glfwApi = GlfwProvider.GLFW.Value;
        WindowPtr = _glfwApi.CreateWindow(800, 600, title, null, null);

        _glfwApi.SetWindowSizeCallback(WindowPtr, (_, width, height) =>
            SizeChange?.Invoke(this, new Size(width, height))
        );
        
        Title = title;
    }

    public void Show() => _glfwApi.ShowWindow(WindowPtr);
    public void Hide() => _glfwApi.HideWindow(WindowPtr);
}

