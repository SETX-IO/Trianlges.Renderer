using System;
using SharpGen.Runtime;
using System.Runtime.CompilerServices;
using Vortice;
using Vortice.DXGI;
using Vortice.Direct3D11;

namespace Triangles.Renderer.Backend.Direct3D11;

public enum BufferUsage
{
    Vertex = 1,
    Index = 2,
    Constant = 4
}

public class DxBuffer
{
    private ID3D11Buffer _buffer;
    private uint _elementSize;
    
    
    public uint BufferSize;
    public BufferUsage BufferUsage;
    
    public DxBuffer(ID3D11Device device, BufferUsage bufferUsage, int bufferSize)
    {
        BufferSize = (uint)bufferSize;
    }

    public void Bind(ID3D11DeviceContext context, uint slot = 0,  ShaderType type = ShaderType.Vertex)
    {
        switch (BufferUsage)
        {
            case BufferUsage.Vertex:
                const uint offset = 0;
                context.IASetVertexBuffers(slot, [_buffer], [_elementSize], [offset]);
            
                break;
            case BufferUsage.Index:
                context.IASetIndexBuffer(_buffer, Format.R32_UInt, 0);
            
                break;
            case BufferUsage.Constant:
                SetConstantBuffer(context, type, slot);
                
                break;
            default:
                throw new ArgumentException($"{nameof(BufferUsage)} is not buffer Type.");
        }
    }
    
    public unsafe void Update<T>(ID3D11DeviceContext context, T[] date)
    {
        if (date.Length * Unsafe.SizeOf<T>() > BufferSize)
            throw new IndexOutOfRangeException($"{nameof(date)} 大于Buffer的大小.");

        _elementSize = (uint)Unsafe.SizeOf<T>();
        
        MappedSubresource map = context.Map(_buffer, MapMode.WriteDiscard);
        Unsafe.Copy(map.DataPointer.ToPointer(), ref date.GetReferenceUnsafe());
        context.Unmap(_buffer);
    }
    
    private void SetConstantBuffer(ID3D11DeviceContext context, ShaderType type, uint slot)
    {
        switch (type)
        {
            case ShaderType.Vertex:
                context.VSSetConstantBuffer(slot, _buffer);
                break;
            case ShaderType.Hull:
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

public class BufferDx11<T> : IBuffer<T> where T : unmanaged
{
    private ID3D11Buffer _buffer;
    
    private bool _isDynamic;
    public readonly uint ElementSize;
    private BindFlags _bufferType;
    
    public BufferDx11(ID3D11Device device, BindFlags bufferType, T[]? data = null)
    {
        _isDynamic = data == null;
        _bufferType = bufferType;
        ResourceUsage usage = _isDynamic ? ResourceUsage.Dynamic : ResourceUsage.Immutable;
        CpuAccessFlags access = _isDynamic ? CpuAccessFlags.Write : CpuAccessFlags.None;

        ElementSize = (uint)Unsafe.SizeOf<T>();
        uint bufferSize = data == null ? ElementSize : (uint)(data.Length * ElementSize);
        
        BufferDescription desc = new(bufferSize, bufferType, usage, access);
        if (data != null)
        {
            DataStream dataStream = DataStream.Create(data, true, true);
            _buffer = device.CreateBuffer(desc, dataStream);
        }
        else
            _buffer = device.CreateBuffer(desc);
    }
    
    public BufferDx11(ID3D11Device device, BindFlags bufferType, uint bufferSize)
    {
        _isDynamic = true;
        _bufferType = bufferType;
        ElementSize = (uint)Unsafe.SizeOf<T>();
        
        BufferDescription desc = new(ElementSize * bufferSize, bufferType, ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _buffer = device.CreateBuffer(desc);
    }

    public unsafe void Update(ID3D11DeviceContext context, T[] data, uint slot = 0, ShaderType type = ShaderType.Vertex)
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
        
        if (!_isDynamic)
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
                context.IASetVertexBuffers(slot, [_buffer], [ElementSize], [offset]);
            
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
            case ShaderType.Hull:
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