using System;
using Hexa.NET.ImGui;
using Trianlges.Graphics;
using Trianlges.Graphics.Direct3D11;
using Trianlges.Renderer.ImGuiImp;
using Vortice.Direct3D11;

namespace Trianlges;

public class GameApp : Application
{
    private readonly Device3D _device;
    private readonly IRenderer _renderer;

    private readonly RendererDx11 _imGui;
    // private readonly IRenderer _d2DRenderer;

    public GameApp(Window mainWindow, string[] args) : base(mainWindow, args)
    {
        _device = new Device3D(MainWindow.Win32Handler);
        
        _renderer = new Renderer.Renderer(_device);
        _imGui =  new RendererDx11(_device);
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
                .Build<Material>();
        trianlgeModule.Material.SetTexture("Assets/image.jpg");
        
        var quadilateralModule = Mesh.Cube;
        quadilateralModule.Material = 
            Material.Create(_device)
                .SetShader("Assets/Shader.hlsl",
                    VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3, VertextType.Uv))
                .Build<Material>();

        var module = Mesh.Trianlge;
        
        if (_renderer is not Renderer.Renderer renderer) return;
        
        renderer.AddDrawElement(trianlgeModule);
        renderer.AddDrawElement(quadilateralModule);
        renderer.AddDrawElement(module);
    }

    private void TryCatchException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = (Exception)e.ExceptionObject;
        Console.WriteLine(exception.Message);
    }

    private void OnChangeSize(IntPtr window, int width, int height)
    {
        Window.AspectRatio = (float)width / height;
        
        if (_renderer is Renderer.Renderer renderer)
            renderer.Camera.Update();
        
        _device.ResetSize((uint)width, (uint)height);
    }

    protected override void Render()
    {
        _renderer.Updata();
        _renderer.Render();
        
        // ImGui.ShowDemoWindow();
        // _imGui.Render();
        
        // _d2DRenderer.Render();
        // _d2DRenderer.Updata();
        
        _device.Present();
    }
}