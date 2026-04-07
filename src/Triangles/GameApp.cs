using System;
using Microsoft.Extensions.Configuration;
using Triangles.Renderer;
using Triangles.Renderer.Backend;
using Triangles.Renderer.Backend.Direct3D11;

namespace Triangles;

public class GameApp : Application
{
    private readonly DeviceDx11 _device;
    private readonly Renderer.Renderer _renderer;
    private readonly SwapChainDx _swapChainDx;

    public GameApp(string[] args) : base(null, args)
    {
        IConfigurationRoot config = new ConfigurationBuilder().
            AddJsonFile("Properties/Develop.json").
            Build();

        MainWindow = new Window(config.GetValue<int>("Window:Width"), config.GetValue<int>("Window:Height"), config["Window:Title"]);
        
        _device = new DeviceDx11();
        _swapChainDx = MainWindow.CreateSwapChain(_device.Device);
        _renderer = new Renderer.Renderer(_device);
    }

    protected override void Initializer(string[] args)
    {
        MainWindow.ChangeSize += OnChangeSize;
        // AppDomain.CurrentDomain.UnhandledException += TryCatchException;

        var trianlgeModule = Mesh.Trianlge;
        trianlgeModule.Program = _device.NewProgram("Assets/Shader.hlsl", "Assets/Shader.hlsl");
        // trianlgeModule.Texture = _device.NewTexture("Assets/image.jpg");
        
        // var quadilateralModule = Mesh.Cube;
        // quadilateralModule.Program = _device.NewProgram("Assets/Shader.hlsl", "Assets/Shader.hlsl");

        // var module = Mesh.Trianlge;
        
        _renderer.AddDrawElement(trianlgeModule);
        _renderer.AddDrawElement(trianlgeModule);
        // _renderer.AddDrawElement(module);
    }

    // private void TryCatchException(object sender, UnhandledExceptionEventArgs e)
    // {
    //     var exception = (Exception)e.ExceptionObject;
    //     Console.WriteLine(exception.Message);
    // }

    private void OnChangeSize(IntPtr window, int width, int height)
    {
        Window.AspectRatio = (float)width / height;
        
        _renderer.Camera.Update();
    }

    protected override void Render()
    {
        _swapChainDx.ClearScreen(_device.Command, Camera.ClearColor);

        CommandBufferDx11 cmdBuffer = CommandPool.Get();
        cmdBuffer.SetViewPort(MainWindow.GetViewport());
        _device.Command.RSSetViewport(MainWindow.GetViewport());
        
        CommandPool.Release(cmdBuffer);
    
        
        _renderer.Update();
        _renderer.Render();
        
        // ImGui.ShowDemoWindow();
        // _imGui.Render();
        
        // _device.Present();
        
        _swapChainDx.Present();
        _device.Submit();
    }
}