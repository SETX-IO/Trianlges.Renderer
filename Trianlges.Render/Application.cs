using System.Runtime.InteropServices;
using GLFW;
using Trianlges.Render.Graphics.Direct3D11;

namespace Trianlges.Render;

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
        
        var d3dDevice = new D3DDevice();
        
        d3dDevice.Create(_window.Win32Handler);
        d3dDevice.CreateRenderResouce();
        
        var renderer = new Renderer(d3dDevice);
        
        while (!Glfw.WindowShouldClose(_window))
        {
            renderer.Render();
            d3dDevice.Present();
            
            Glfw.PollEvents();
        }
    }

    private void Initializer()
    {

    }
}