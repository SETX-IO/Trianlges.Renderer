using System;
using System.Drawing;
using System.Runtime.Versioning;
using GLFW;

namespace Trianlges.Render;

public class Window : IDisposable
{
    public static float AspectRatio;
    private readonly GLFW.Window _window;
    private Size _size;
    private string _title;
    [SupportedOSPlatform("Windows")] private IntPtr _win32Handler;
    
    /// <summary>
    ///     Window's title.
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
    ///     Window's size.
    /// </summary>
    public Size Size
    {
        get => _size;
        set
        {
            if (Equals(value, Size)) return;

            _size = value;
            AspectRatio = (float)value.Width/ value.Height;
            Glfw.SetWindowSize(_window, value.Width, value.Height);
        }
    }

    public bool IsClose => Glfw.WindowShouldClose(_window);

    /// <summary>
    ///     Window's HWND.
    /// </summary>
    [SupportedOSPlatform("Windows")]
    public IntPtr Win32Handler
    {
        get
        {
            if (_win32Handler == IntPtr.Zero) _win32Handler = Native.GetWin32Window(_window);

            return _win32Handler;
        }
    }

    private SizeCallback? _changeSizeCallBack;
    public event SizeCallback? ChangeSize
    {
        add
        {
            if (value == null) return;
            
            _changeSizeCallBack = value;
            Glfw.SetFramebufferSizeCallback(_window, _changeSizeCallBack);
        }
        remove
        {
            if (value == _changeSizeCallBack)
                _changeSizeCallBack = null;
            Glfw.SetFramebufferSizeCallback(_window, _changeSizeCallBack);
        }
    }
        
    /// <summary>
    ///     Create Window.
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
    public void Dispose() => Glfw.DestroyWindow(_window);
}