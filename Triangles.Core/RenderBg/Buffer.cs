using System.Runtime.CompilerServices;
using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public class Buffer<T> where T : unmanaged
{
    private readonly ID3D12Resource _buffer;
    private readonly Device _device;
    private readonly uint _bufferSize;
    private readonly uint _itemCount;
    public ResourceStates Type { get; }

    public VertexBufferView VertexBufferView => Type == ResourceStates.VertexAndConstantBuffer
        ? new VertexBufferView(_buffer.GPUVirtualAddress, _bufferSize, (uint)Unsafe.SizeOf<T>())
        : throw new TypeAccessException("");
    
    public IndexBufferView IndexBufferView => Type == ResourceStates.IndexBuffer
        ? new IndexBufferView(_buffer.GPUVirtualAddress, _bufferSize, new T() is uint)
        : throw new TypeAccessException();

    public Buffer(Device device, uint itemCount, ResourceStates bufferType)
    {
        _bufferSize = (uint)Unsafe.SizeOf<T>() * itemCount;

        _itemCount = itemCount;
        Type = bufferType;
        _device = device;
        
        _buffer = device.LDevice.CreateCommittedResource<ID3D12Resource>(new HeapProperties(HeapType.Upload), HeapFlags.None,
            ResourceDescription.Buffer(_bufferSize), bufferType);
    }

    public void SetData(params T[]? data)
    {
        if (data != null)
            _buffer.SetData(data);
    }
    
    public  Span<T> GetMapSpan()
    {
        return _buffer.Map<T>(0, 1);
    }

    public void ToConstantBuffer(ShaderVisibility bindShader, uint bindSlot = 0)
    {
        UpLoadHeap heap = new UpLoadHeap(_device.DescriptionHeapPool.GetHeap(DescriptionHeapType.CbvSrvUav));
        
        ConstantBufferViewDescription? cBufferInfo = new(_buffer.GPUVirtualAddress, 256);
        
        heap.CreateCbv(cBufferInfo, ShaderVisibility.Vertex, bindSlot);

        // return descHeap;
    }
}