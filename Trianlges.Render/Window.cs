using System;
using System.Drawing;
using System.Runtime.Versioning;
using GLFW;

namespace Trianlges.Render;

public class Window : IDisposable
{
    private readonly GLFW.Window _window;
    private string _title;
    private Size _size;
    [SupportedOSPlatform("Windows")] private IntPtr _win32Handler;

    public static float AspectRatio;
    
    /// <summary>
    /// Window's title.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (Equals(value, _title)) return;

            _title = value;
            Glfw.SetWindowTitle(_window, value);
        }
    }
    
    /// <summary>
    /// Window's size.
    /// </summary>
    public Size Size
    {
        get => _size;
        set
        {
            if (Equals(value, Size)) return;

            _size = value;
            Glfw.SetWindowSize(_window, value.Width, value.Height);
        }
    }

    public bool IsClose => Glfw.WindowShouldClose(_window);

    /// <summary>
    /// Window's HWND.
    /// </summary>
    [SupportedOSPlatform("Windows")]
    public IntPtr Win32Handler {
        get
        {
            if (_win32Handler == IntPtr.Zero)
            {
                _win32Handler = Native.GetWin32Window(_window);
            }

            return _win32Handler;
        }
    }

    /// <summary>
    /// Create Window.
    /// </summary>
    /// <param name="width">width</param>
    /// <param name="height">height</param>
    /// <param name="title">title</param>
    public Window(int width, int height, string title)
    {
        _window = Glfw.CreateWindow(width, height, title, default, default);
        _title = title;
        _size = new Size(width, height);
        AspectRatio = (float)width / height;
    }

    public void DispatchMessage() => Glfw.PollEvents();
    
    public void Dispose()
    {
        Glfw.DestroyWindow(_window);
    }
}