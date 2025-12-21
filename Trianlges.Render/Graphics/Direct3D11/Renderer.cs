using System.Collections.Generic;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Renderer : IRenderer
{
    private readonly D3DDevice? _device;
    private readonly List<DrawElement> _drawElements;

    public Renderer(D3DDevice? device)
    {
        _device = device;
        _drawElements = [];
    }

    public void AddDrawElement(DrawElement? element)
    {
        if (element == null) return;
        
        _drawElements.Add(element);
    }
    
    public void Render()
    {
        if (_device == null) return;
        
        var renderTarget = _device.RenderTarget!;
        var context = _device.DContext!;

        var clearColor = new Color(0, 50, 100);
        
        context.ClearRenderTargetView(renderTarget, clearColor);

        foreach (var drawElement in _drawElements)
        {
            drawElement.Render(_device);
        }
    }
}