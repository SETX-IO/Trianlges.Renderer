using System.Collections.Generic;
using System.Numerics;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Renderer : IRenderer
{
    private readonly D3DDevice _device;
    private readonly List<DrawElement> _drawElements;
    
    private readonly Camera _camera;

    private ID3D11Buffer? _contextBuffer;
    
    public Renderer(D3DDevice device)
    {
        _device = device;
        _drawElements = [];
        
        _camera = new Camera(new Vector3(0, 0, -1.5f), 45);
    }

    public void AddDrawElement(DrawElement? element)
    {
        if (element == null) return;
        
        _drawElements.Add(element);
    }
    
    private float _index;
    
    public void Updata()
    {
        if (_contextBuffer == null)
        {
            var cBufferDesc = new BufferDescription(ConstantBufferData.Size, BindFlags.ConstantBuffer);
            _contextBuffer = _device.Device.CreateBuffer(cBufferDesc);
        }
        
        _index += 0.0005f;
        var rotation = Matrix4x4.CreateRotationY(_index);
        var rotation1 = Matrix4x4.CreateRotationX(_index);
        // var translation = Matrix4x4.CreateTranslation(MathF.Sin(_index) * 12, 0, MathF.Cos(_index) * 25);
        var translation = Matrix4x4.CreateTranslation(0, 0, 2);

        // _camera.Position = new Vector3(MathF.Sin(_index) * 12, 0, MathF.Cos(_index) * 25);
        var data = new ConstantBufferData(_camera.Proj * _camera.View * translation);
        
        _device.DContext.UpdateSubresource([data], _contextBuffer);
    }

    public void Render()
    {
        var renderTarget = _device.RenderTarget;
        var depthStencil = _device.DepthStencil;
        var context = _device.DContext;

        var clearColor = new Color(0, 50, 100);
        context.ClearRenderTargetView(renderTarget, clearColor);
        context.ClearDepthStencilView(depthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
        
        context.VSSetConstantBuffers(0, [_contextBuffer]);
        
        _drawElements.ForEach(drawElement => drawElement.Render(_device));
    }

    public void Release()
    {
        _device.Release();

        _drawElements.ForEach(drawElement => drawElement.Release());
        
        _drawElements.Clear();
    }
}