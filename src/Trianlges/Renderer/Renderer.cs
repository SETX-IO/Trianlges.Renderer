using System;
using System.Collections.Generic;
using System.Numerics;
using Trianlges.Graphics;
using Trianlges.Graphics.Direct3D11;
using Vortice.Direct3D11;

namespace Trianlges.Renderer;

public class Renderer : IRenderer
{
    /// <summary>
    /// SRT矩阵传入前需要进行转置.
    /// </summary>
    private ConstantBufferData _constantData;
    private float _index;
    private readonly Device3D _device;
    private readonly List<DrawElement> _drawElements;
    private readonly IBuffer<ConstantBufferData> _cBuffer;
    
    public readonly Camera Camera;
    
    public Renderer(Device3D device)
    {
        _device = device;
        _drawElements = [];

        Camera = new Camera(new Vector3(0, 0, -2));
        _constantData = new ConstantBufferData(Camera.View, Camera.Proj);

        _cBuffer = device.NewBuffer<ConstantBufferData>(BindFlags.ConstantBuffer,isDynamic: true);

    }
    
    public void Updata()
    {
        var context = _device.DContext;
        _cBuffer.Update(context, [_constantData]);
    }

    private bool _isA;

    public void Render()
    {
        _device.Clear();
        
        _index += Time.DetalTime * 3f;

        foreach (var element in _drawElements)
        {
            _device.RenderPipeLine.WireFrameEnable = !_isA;
            if (_isA)
            {
                element.Transform.SetRotation(0, _index, _index);
                element.Transform.SetPosition(0, MathF.Cos(_index), 5);
            
                element.Render(_device);
                _isA = !_isA;
                
                return;
            }
            element.Transform.SetRotation(0, -_index, _index);
            element.Transform.SetPosition(0, MathF.Sin(-_index), 5);
            
            element.Render(_device);
            _isA = !_isA;
        }
    }

    public void AddDrawElement(DrawElement? element)
    {
        if (element == null) return;

        _drawElements.Add(element);
    }
}