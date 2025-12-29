using System;
using Trianlges.Render.Graphics;
using Trianlges.Render.Graphics.Direct3D11;
using Trianlges.Render.Util;

namespace Trianlges.Render;

public class GameApp : Application
{
    private readonly D3DDevice _device;
    private readonly IRenderer _renderer;
    private readonly IRenderer _d2DRenderer;

    public GameApp(Window mainWindow, string[] args) : base(mainWindow, args)
    {
        _device = new D3DDevice(MainWindow.Win32Handler);
        
        _renderer = new Graphics.Direct3D11.Renderer(_device);

        // var d2dRender = new Graphics.Direct2D.Renderer(_device);
        // d2dRender.Init();
        //
        // _d2DRenderer = d2dRender;
    }

    protected override void Initializer(string[] args)
    {
        MainWindow.ChangeSize += OnChangeSize;

        var trianlgeModule = Mesh.Quadrilateral;
        // var quadilateralModule = Mesh.Cube;

        if (_renderer is not Renderer renderer) return;
        
        renderer.AddDrawElement(trianlgeModule);
        // renderer.AddDrawElement(quadilateralModule);
    }

    private void OnChangeSize(IntPtr window, int width, int height)
    {
        Window.AspectRatio = (float)width / height;
        
        if (_renderer is Renderer renderer)
            renderer.Camera.Updata();
        
        _device.ResetSize((uint)width, (uint)height);
    }

    protected override void Render()
    {
        _renderer.Updata();
        _renderer.Render();
        
        // _d2DRenderer.Render();
        // _d2DRenderer.Updata();
        
        _device.Present();
    }
}