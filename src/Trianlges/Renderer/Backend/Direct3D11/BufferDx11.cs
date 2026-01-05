using System;
using SharpGen.Runtime;
using System.Runtime.CompilerServices;
using Vortice;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class BufferDx11<T> where T : unmanaged
{
    private ID3D11Buffer _buffer;
    private bool _isDyamic;
    private BindFlags _bufferType;

    private BufferDx11() { throw new NotImplementedException(); }

    public BufferDx11(ID3D11Device device, BindFlags bufferType, T[]? data = null, bool isByamic = false)
    {
        _isDyamic = isByamic;
        _bufferType = bufferType;
        ResourceUsage usage = isByamic ? ResourceUsage.Dynamic : ResourceUsage.Immutable;
        CpuAccessFlags access = isByamic ? CpuAccessFlags.Write : CpuAccessFlags.None;

        uint bufferSize = data == null ? (uint)Unsafe.SizeOf<T>() : (uint)(data.Length * Unsafe.SizeOf<T>());
        
        BufferDescription desc = new(bufferSize, bufferType, usage, access);
        if (data != null)
        {
            DataStream dataStream = DataStream.Create(data, true, true);
            _buffer = device.CreateBuffer(desc, dataStream);
        }
        else
        {
            _buffer = device.CreateBuffer(desc);
        }

    }

    public unsafe void Updata(ID3D11DeviceContext context, T[] data, uint slot = 0, ShaderType type = ShaderType.Vertex)
    {
        switch (_bufferType)
        {
            case BindFlags.VertexBuffer or BindFlags.IndexBuffer:
                Bind(context, slot);
                break;
            case BindFlags.ConstantBuffer:
                SetConstantBuffer(context, type, slot);
                break;
            default:
                throw new ArgumentException($"{nameof(_bufferType)} is not buffer Type.");
        }
        
        if (!_isDyamic)
            context.UpdateSubresource(data, _buffer);
        
        MappedSubresource map = context.Map(_buffer, MapMode.WriteDiscard);
            
        Unsafe.Copy(map.DataPointer.ToPointer(), ref data.GetReferenceUnsafe());
            
        context.Unmap(_buffer);
    }

    public void Bind(ID3D11DeviceContext context, uint slot = 0)
    {
        switch (_bufferType)
        {
            case BindFlags.VertexBuffer:
                const uint offset = 0;
                uint stride = (uint)Unsafe.SizeOf<T>();
                context.IASetVertexBuffers(slot, [_buffer], [stride], [offset]);
            
                break;
            case BindFlags.IndexBuffer:
                context.IASetIndexBuffer(_buffer, Format.R32_UInt, 0);
            
                break;
            default:
                throw new ArgumentException($"{nameof(_bufferType)} is not buffer Type.");
        }
    }

    private void SetConstantBuffer(ID3D11DeviceContext context, ShaderType type, uint slot)
    {
        switch (type)
        {
            case ShaderType.Vertex:
                context.VSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Hell:
                context.HSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Domain:
                context.DSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Geometry:
                context.GSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Pixel:
                context.PSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Compute:
                context.CSSetConstantBuffer(slot, _buffer);
                break;
        }
    }
}