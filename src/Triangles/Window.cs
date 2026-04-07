using System;
using System.Drawing;
using System.Runtime.Versioning;
using GLFW;

namespace Triangles;

public enum VideoType
{
    OpenGl,
    OpenGLes,
    Vulkan,
    DirectX9,
    DirectX11,
    DirectX12,
}

public class Window : IDisposable
{
    public static float AspectRatio;
    private readonly GLFW.Window _window;
    private Size _size;
    private string _title;
    [SupportedOSPlatform("Windows")] private IntPtr _win32Handler;

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
    
    public Size Size
    {
        get
        {
            Glfw.GetWindowSize(_window, out int width, out int height);
            _size = new Size(width, height);
            
            return _size;
        } 
        set
        {
            if (Equals(value, Size)) return;

            _size = value;
            AspectRatio = (float)value.Width/ value.Height;
            Glfw.SetWindowSize(_window, value.Width, value.Height);
        }
    }

    public bool IsClose => Glfw.WindowShouldClose(_window);
    
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
        

    public Window(int width, int height, string? title)
    {
        _title = title ?? "Glfw Window";
        _size = new Size(width, height);
        
        _window = Glfw.CreateWindow(width, height, title, default, default);
        
        AspectRatio = (float)width / height;
    }
    
    public void DispatchMessage() => Glfw.PollEvents();
    public void Dispose() => Glfw.DestroyWindow(_window);
}