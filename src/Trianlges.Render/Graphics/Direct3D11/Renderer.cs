using System.Numerics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Renderer : IRenderer
{
    public readonly Camera Camera;
    private readonly IDevice3D _device;
    private readonly List<DrawElement> _drawElements;

    private ID3D11Buffer? _contextBuffer;
    private ConstantBufferData _constantData;

    private float _index;

    public Renderer(IDevice3D device)
    {
        _device = device;
        _drawElements = [];

        Camera = new Camera(new Vector3(0, 0, -2));
        _constantData = new ConstantBufferData(Matrix4x4.Identity, Camera.View, Camera.Proj);
    }

    public unsafe void Updata()
    {
        if (_contextBuffer == null)
        {
            var cBufferDesc = new BufferDescription(ConstantBufferData.Size, BindFlags.ConstantBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);
            _contextBuffer = _device.Device.CreateBuffer(cBufferDesc);
            
            _device.DContext.VSSetConstantBuffers(0, [_contextBuffer]);
        }

        _index += 0.0005f;
        var rotation = Matrix4x4.CreateRotationY(_index);
        var translation = Matrix4x4.CreateTranslation(0, 0, 2);
        var scale = Matrix4x4.CreateScale(0.4f);
        
        // SRT矩阵传入前需要进行转置.
        _constantData.Module = Matrix4x4.Transpose(rotation * translation);
        
        var map = _device.DContext.Map(_contextBuffer, MapMode.WriteDiscard);
        Unsafe.Copy(map.DataPointer.ToPointer(), ref _constantData);
        _device.DContext.Unmap(_contextBuffer);
    }

    public void Render()
    {
        var renderTarget = _device.RenderTarget;
        var depthStencil = _device.DepthStencil;
        var context = _device.DContext;
        
        context.ClearRenderTargetView(renderTarget, Camera.ClearColor);
        context.ClearDepthStencilView(depthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f,
            0);
        
        _drawElements.ForEach(drawElement => drawElement.Render(_device));
    }

    public void AddDrawElement(DrawElement? element)
    {
        if (element == null) return;

        _drawElements.Add(element);
    }
}