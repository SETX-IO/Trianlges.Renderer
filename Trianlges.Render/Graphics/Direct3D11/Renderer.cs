using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Renderer : IRenderer
{
    private readonly D3DDevice? _device;

    public Renderer(D3DDevice? device)
    {
        _device = device;
    }
    
    public void Render()
    {
        if (_device == null) return;
        
        var renderTarget = _device.RenderTarget!;
        var context = _device.DContext!;

        var clearColor = new Color(0, 50, 100);
        context.ClearRenderTargetView(renderTarget, clearColor);
    }
}