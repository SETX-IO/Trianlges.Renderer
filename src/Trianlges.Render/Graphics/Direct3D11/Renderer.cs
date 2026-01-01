using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Trianlges.Render.Module;
using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Renderer : IRenderer
{
    public readonly Camera Camera;
    private readonly IDevice3D _device;
    private readonly List<DrawElement> _drawElements;
    private readonly ID3D11DeviceContext _context;

    private readonly ID3D11Buffer _contextBuffer;
    
    /// <summary>
    /// SRT矩阵传入前需要进行转置.
    /// </summary>
    private ConstantBufferData _constantData;

    private float _index;

    public Renderer(IDevice3D device)
    {
        _device = device;
        _drawElements = [];

        Camera = new Camera(new Vector3(0, 0, -2));
        _constantData = new ConstantBufferData(Camera.View, Camera.Proj);
        
        var cBufferDesc = new BufferDescription(ConstantBufferData.Size, BindFlags.ConstantBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _contextBuffer = _device.Device.CreateBuffer(cBufferDesc);
            
        // _context = device.Device.CreateDeferredContext();
        _device.DContext.VSSetConstantBuffers(0, [_contextBuffer]);
    }
    
    public unsafe void Updata()
    {
        var context = _device.DContext;
        
        var map1 = context.Map(_contextBuffer, MapMode.WriteDiscard);
        Unsafe.Copy(map1.DataPointer.ToPointer(), ref _constantData);
        context.Unmap(_contextBuffer);
    }

    private bool isA = false;

    public unsafe void Render()
    {
        var renderTarget = _device.RenderTarget;
        var depthStencil = _device.DepthStencil;
        var context = _device.DContext;
        
        context.ClearRenderTargetView(renderTarget, Camera.ClearColor);
        context.ClearDepthStencilView(depthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f,
            0);

        // _context.FinishCommandList(false, out var list);
        // context.ExecuteCommandList(list, true);
        
        _index += 0.0002f;

        foreach (var element in _drawElements)
        {
            if (isA)
            {
                element.Transfome.SetRotation(0, _index, _index);
                element.Transfome.SetPosition(0, MathF.Cos(_index), 5);
            
                element.Render(_device);
                isA = !isA;
                
                return;
            }
            element.Transfome.SetRotation(0, -_index, _index);
            element.Transfome.SetPosition(0, MathF.Sin(-_index), 5);
            
            element.Render(_device);
            
            isA = !isA;
        }
    }

    public void AddDrawElement(DrawElement? element)
    {
        if (element == null) return;

        _drawElements.Add(element);
    }
}