using System;
using Trianlges.Render.Graphics;
using Trianlges.Render.Graphics.Direct3D11;
using Vortice.Direct3D11;

namespace Trianlges.Render;

public class GameApp : Application
{
    private readonly D3DDevice _device;
    private readonly IRenderer _renderer;
    // private readonly IRenderer _d2DRenderer;

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
        AppDomain.CurrentDomain.UnhandledException += TryCatchException;

        var trianlgeModule = Mesh.Quadrilateral;
        trianlgeModule.Material =
            Material.Create(_device)
                .SetShader("Assets/TextureShader.hlsl",
                    VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3, VertextType.Uv))
                .SetTexture("Assets/image.jpg")
                .ConfigRasterizer(false, false)
                .Build<Material>();
        
        var quadilateralModule = Mesh.Cube;
        quadilateralModule.Material = 
            Material.Create(_device)
                .SetShader("Assets/Shader.hlsl",
                    VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3, VertextType.Uv))
                .ConfigRasterizer(false, true)
                .Build<Material>();

        if (_renderer is not Renderer renderer) return;
        
        renderer.AddDrawElement(trianlgeModule);
        renderer.AddDrawElement(quadilateralModule);
    }

    private void TryCatchException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = (Exception)e.ExceptionObject;
        Console.WriteLine(exception.Message);
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