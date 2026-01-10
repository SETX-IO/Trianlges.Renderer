using System;
using Trianlges.Logger;
using Trianlges.Renderer;
using Trianlges.Renderer.Backend.Direct3D11;

namespace Trianlges;

public class GameApp : Application
{
    private readonly DeviceDx11 _device;
    private readonly Renderer.Renderer _renderer;

    public GameApp(Window mainWindow, string[] args) : base(mainWindow, args)
    {
        _device = new DeviceDx11(MainWindow.Win32Handler);
        
        _renderer = new Renderer.Renderer(_device);
    }

    protected override void Initializer(string[] args)
    {
        MainWindow.ChangeSize += OnChangeSize;
        AppDomain.CurrentDomain.UnhandledException += TryCatchException;

        var trianlgeModule = Mesh.Quadrilateral;
        trianlgeModule.Program = _device.NewProgram("Assets/TextureShader.hlsl", "Assets/TextureShader.hlsl");
        trianlgeModule.Texture = _device.NewTexture("Assets/image.jpg");
        
        var quadilateralModule = Mesh.Cube;
        quadilateralModule.Program = _device.NewProgram("Assets/Shader.hlsl", "Assets/Shader.hlsl");

        // var module = Mesh.Trianlge;
        
        _renderer.AddDrawElement(trianlgeModule);
        _renderer.AddDrawElement(quadilateralModule);
        // _renderer.AddDrawElement(module);
    }

    private void TryCatchException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = (Exception)e.ExceptionObject;
        Console.WriteLine(exception.Message);
    }

    private void OnChangeSize(IntPtr window, int width, int height)
    {
        Window.AspectRatio = (float)width / height;
        
        _renderer.Camera.Update();
        _device.ResetSize((uint)width, (uint)height);
    }

    protected override void Render()
    {
        _renderer.Update();
        _renderer.Render();
        
        // ImGui.ShowDemoWindow();
        // _imGui.Render();
        
        _device.Present();
    }
}