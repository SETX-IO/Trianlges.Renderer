using System;
using System.Drawing;
using GLFW;

namespace Triangles;

[Flags]
public enum WindowState
{
    None,
    NoResize,
    NoError
}

public abstract class Application
{
    private readonly string[] _args;
    protected Window? MainWindow;

    protected Application(Window? mainWindow, string[] args)
    {
        MainWindow = mainWindow;
        _args = args;
    }

    public void Run()
    {
        Initializer(_args);

        while (!MainWindow.IsClose)
        {
            if (MainWindow.Size != Size.Empty)
            {
                Time.Update();
            
                Render();
            }
            
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
    
    public static void Init(VideoType videoType = VideoType.OpenGl, WindowState windowState = WindowState.None)
    {
        SetVideoApi(videoType);
        SetWindowState(windowState);
    }
    
    private static void SetVideoApi(VideoType videoType)
    {
        switch (videoType)
        {
            case VideoType.OpenGl:
                Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
                break;
            case VideoType.OpenGLes:
                Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGLES);
                break;
            case VideoType.Vulkan:
            case VideoType.DirectX9:
            case VideoType.DirectX11:
            case VideoType.DirectX12:
                Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(videoType), videoType, null);
        }
    }
    
    private static void SetWindowState(WindowState windowState)
    {
        if ((windowState & WindowState.NoResize) != WindowState.None)
        {
            Glfw.WindowHint(Hint.Resizable, false);
        }

        if ((windowState & WindowState.NoError) != WindowState.None)
        {
            Glfw.WindowHint(Hint.ContextNoError, true);
        }
    }
}