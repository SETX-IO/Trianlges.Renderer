using Silk.NET.GLFW;

namespace Triangles.BaseApp;

public unsafe struct WindowsHwndPoint
{
    public readonly nint Handel;
    
    public WindowsHwndPoint(Window window)
    {
        Glfw glfwApi = GlfwProvider.GLFW.Value;
        GlfwNativeWindow nativeWindow = new GlfwNativeWindow(glfwApi, window.WindowPtr);
        
        if (nativeWindow.Win32.HasValue)
            Handel = nativeWindow.Win32.Value.Hwnd;
    }
}