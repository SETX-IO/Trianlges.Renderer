using System;
using Trianlges.Render.Graphics;
using Trianlges.Render.Graphics.Direct3D11;
using ImGuiNET;

namespace Trianlges.Render;

public class GameApp : Application
{
    private readonly D3DDevice _device = new();
    private readonly IRenderer _renderer;
    // private readonly IRenderer _imGuiRenerer;

    public GameApp(Window mainWindow, string[] args) : base(mainWindow, args)
    {
        _renderer = new Renderer(_device);
        // _imGuiRenerer = new ImGuiRenderer(_device);
    }

    protected override void Initializer(string[] args)
    {
        MainWindow.ChangeSize += OnChangeSize;
        
        _device.Create(MainWindow.Win32Handler);
        _device.ConfigRenderTarget();

        var trianlgeModule = Module.Quadrilateral;
        if (_renderer is Renderer renderer)
            renderer.AddDrawElement(trianlgeModule);

        // var guiContxet = ImGui.CreateContext();
        // ImGui.SetCurrentContext(guiContxet);
        // ImGui.ShowDemoWindow();
        // ImDrawDataPtr a = ImGui.GetDrawData();
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
        // ImGui.NewFrame();
        //
        // ImGui.Render();
        
        _renderer.Updata();
        _renderer.Render();

        _device.Present();
    }
}