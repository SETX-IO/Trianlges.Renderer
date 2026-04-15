using Triangles.Core.Renderer;
using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public enum DescriptionHeapType
{
    Rtv,
    CbvSrvUav,
}

public class DescriptionHeap(Device device)
{
    public Device Device => device;
    public required ID3D12DescriptorHeap Heap;
    public uint IncrementSize;
    public int Count;
    
    public CpuDescriptorHandle CpuHandle => Heap.GetCPUDescriptorHandleForHeapStart();
    public GpuDescriptorHandle GpuHandle => Heap.GetGPUDescriptorHandleForHeapStart();
}

public class UpLoadHeap
{
    private DescriptionHeap _descriptionHeap;

    private int _offsetCount;

    private CpuDescriptorHandle _currentCbvHandle;
    private CpuDescriptorHandle _currentSrvHandle;
    private CpuDescriptorHandle _currentUavHandle;

    public GpuDescriptorHandle CbvGpuHandle => _descriptionHeap.GpuHandle;
    public GpuDescriptorHandle SrvGpuHandle => _descriptionHeap.GpuHandle.Offset(_offsetCount -1, _descriptionHeap.IncrementSize);
    public GpuDescriptorHandle UavGpuHandle => _descriptionHeap.GpuHandle.Offset(_offsetCount * 2 - 1, _descriptionHeap.IncrementSize);

    public UpLoadHeap(DescriptionHeap descriptionHeap)
    {
        _descriptionHeap = descriptionHeap;

        _offsetCount = _descriptionHeap.Count / 3;

        _currentCbvHandle = _descriptionHeap.CpuHandle;
        _currentSrvHandle = _descriptionHeap.CpuHandle.Offset(_offsetCount - 1, _descriptionHeap.IncrementSize);
        _currentUavHandle = _descriptionHeap.CpuHandle.Offset(_offsetCount * 2 - 1, _descriptionHeap.IncrementSize);
    }
    
    public void CreateCbv(ConstantBufferViewDescription? info, ShaderVisibility shaderVisibility, uint slot = 0)
    {
        _descriptionHeap.Device.LDevice.CreateConstantBufferView(info, _currentCbvHandle);

        _descriptionHeap.Device.AddRootSignatureParameter(shaderVisibility, DescriptorRangeType.ConstantBufferView, slot);
        _currentCbvHandle.Offset(1, _descriptionHeap.IncrementSize);
    }
    
    public void CreateSrv(ID3D12Resource resource, ShaderResourceViewDescription? info, ShaderVisibility shaderVisibility, uint slot = 0)
    {
        _descriptionHeap.Device.LDevice.CreateShaderResourceView(resource, info, _currentSrvHandle);

        _descriptionHeap.Device.AddRootSignatureParameter(shaderVisibility, DescriptorRangeType.ShaderResourceView, slot);
        _currentSrvHandle.Offset(1, _descriptionHeap.IncrementSize);
    }
    
    // public void CreateUav()
    // {
    //     _descriptionHeap.Device.CreateUnorderedAccessView();
    // }
}

public class DescriptionHeapPool
{
    private readonly Device _device;
    private readonly uint _descCount;
    private readonly Dictionary<DescriptionHeapType, DescriptionHeap> _heapCache = new();
    
    public DescriptionHeapPool(Device device, uint descCount = 16)
    {
        _device = device;
        _descCount = descCount;
    }
    
    public DescriptionHeap GetHeap(DescriptionHeapType heapType, uint descCount = 0)
    {
        if (_heapCache.TryGetValue(heapType, out var heap))
        {
            return heap;
        }

        var device = _device.LDevice;

        DescriptorHeapFlags flags =
            heapType is  DescriptionHeapType.CbvSrvUav
                ? DescriptorHeapFlags.ShaderVisible
                : DescriptorHeapFlags.None;

        (DescriptorHeapType, uint) valueTuple = heapType switch
        {
            DescriptionHeapType.Rtv => (DescriptorHeapType.RenderTargetView, _descCount),
            DescriptionHeapType.CbvSrvUav  => (
                    DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView, _descCount * 3),
            _ => throw new ArgumentOutOfRangeException(nameof(heapType), heapType, null)
        };

        DescriptorHeapDescription descHeapInfo = new()
        {
            DescriptorCount = valueTuple.Item2,
            Flags = flags,
            NodeMask = 0,
            Type = valueTuple.Item1
        };

        _heapCache[heapType] = new DescriptionHeap(_device)
        {
            Count = (int)valueTuple.Item2,
            IncrementSize = device.GetDescriptorHandleIncrementSize(valueTuple.Item1),
            Heap = device.CreateDescriptorHeap<ID3D12DescriptorHeap>(descHeapInfo)
        };
        
        return _heapCache[heapType];
    }

    public void BindDescHeap(ICommandBuffer cmd)
    {
        var cmdBuffer = cmd.CmdBuffer.CommandList;
        
        var keyValuePairs = _heapCache.Where(descriptionHeap =>
            descriptionHeap.Key != DescriptionHeapType.Rtv).ToArray();


        foreach (var keyValuePair in keyValuePairs)
        {
            cmdBuffer.SetDescriptorHeaps(keyValuePair.Value.Heap);

            if (keyValuePair.Key != DescriptionHeapType.CbvSrvUav) continue;
            
            UpLoadHeap heap = new(keyValuePair.Value);
                
            uint i = 0;
            cmdBuffer.SetGraphicsRootDescriptorTable(i++, heap.CbvGpuHandle);
            cmdBuffer.SetGraphicsRootDescriptorTable(i++, heap.SrvGpuHandle);
            // cmdBuffer.SetGraphicsRootDescriptorTable(i++, heap.UavGpuHandle);
        }
    }
}