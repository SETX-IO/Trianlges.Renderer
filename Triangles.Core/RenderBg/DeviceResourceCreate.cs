using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public partial class Device
{
    public (ReadOnlyMemory<ID3D12Resource> resources, ID3D12DescriptorHeap heap) CreateRenderTargetViews(uint bufferCount, Func<int, ID3D12Resource> backBufferCallBack)
    {
            
        ID3D12Resource[] resources = new ID3D12Resource[bufferCount];

        uint rtvDescSize = RtvDescriptorSize;
            
        DescriptorHeapDescription desc = new DescriptorHeapDescription(DescriptorHeapType.RenderTargetView, bufferCount);
        ID3D12DescriptorHeap heap = _device.CreateDescriptorHeap<ID3D12DescriptorHeap>(desc);

        CpuDescriptorHandle rtvHandel = heap.GetCPUDescriptorHandleForHeapStart();

        for (int i = 0; i < resources.Length; i++)
        {
            ID3D12Resource backBuffer = backBufferCallBack(i);
                
            _device.CreateRenderTargetView(backBuffer, null, rtvHandel);
            resources[i] = backBuffer;

            rtvHandel.Offset(1, rtvDescSize);
            // rtvHandel.Ptr += rtvDescSize;
        }
            
        return (resources, heap);
    }

    public ID3D12Fence CreateFence(ulong initialValue = 0UL, FenceFlags flags = FenceFlags.None)
    {
        return _device.CreateFence(initialValue, flags);
    }

    public ID3D12PipelineState CreatePipLine(GraphicsPipelineStateDescription pipeLineInfo) =>
        _device.CreateGraphicsPipelineState(pipeLineInfo);
        
    public CommandBufferPool CreateCommandBufferPool(uint allocationCount)
    {
        CommandBufferPool pool = new CommandBufferPool(_device, allocationCount);
        return pool;
    }
}