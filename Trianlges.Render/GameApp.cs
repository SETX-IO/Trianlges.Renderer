using Trianlges.Render.Graphics;
using Trianlges.Render.Graphics.Direct3D11;

namespace Trianlges.Render;

public class GameApp : Application
{
    private readonly D3DDevice _device = new();
    private readonly IRenderer _renderer;
    
    public GameApp(Window window, string[] args) : base(window, args)
    {
        _renderer = new Renderer(_device);
    }

    protected override void Initializer()
    {
        _device.Create(_window.Win32Handler);
        _device.CreateRenderResouce();
        
        var trianlgeModule = Module.Trianlge;
        if (_renderer is Graphics.Direct3D11.Renderer renderer)
            renderer.AddDrawElement(trianlgeModule);
    }

    protected override void Render()
    {
        _renderer.Updata();
        _renderer.Render();
        
        _device.Present();
    }
}