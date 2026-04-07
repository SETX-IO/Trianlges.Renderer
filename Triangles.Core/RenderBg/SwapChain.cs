using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class SwapChain : IDisposable
{
    private IDXGISwapChain4? _swapChain;
    
    private ReadOnlyMemory<ID3D12Resource> _backBuffers;
    private ID3D12DescriptorHeap? _rtvHeap;
    private SwapChainDescription1 _swapChainInfo;
    private uint _rtvDescSize;
    
    public CpuDescriptorHandle CurrentRtv {
        get
        {
            uint index = CurrentIndex;
            CpuDescriptorHandle rtvHandel = _rtvHeap.GetCPUDescriptorHandleForHeapStart();

            rtvHandel.Ptr += index * _rtvDescSize;
            
            return rtvHandel;
        }
    }

    public ID3D12Resource CurrentBackBuffer => _backBuffers.Span[(int)CurrentIndex];
    public uint CurrentIndex => _swapChain.CurrentBackBufferIndex;
    public uint BufferCount => _swapChainInfo.BufferCount;

    public void Create(Device device, nint windowHandel, USize windowSize)
    {
        _swapChainInfo = new SwapChainDescription1(windowSize.Width, windowSize.Height);

        using IDXGISwapChain1 swapChain =
            Dxgi.Factory.Value.CreateSwapChainForHwnd(device.CommandQueue, windowHandel, _swapChainInfo);
        
        _swapChain = swapChain.QueryInterface<IDXGISwapChain4>();
        _rtvDescSize = device.RtvDescriptorSize;
        
        var heapAndBackBuffer = device.CreateRenderTargetViews(BufferCount, i => _swapChain.GetBuffer<ID3D12Resource>((uint)i));
        _rtvHeap = heapAndBackBuffer.heap;
        _backBuffers = heapAndBackBuffer.resources;
    }

    public void Present(bool isSync = false)
    {
        _swapChain?.Present(Convert.ToUInt32(isSync), PresentFlags.None);
    }
    
    public void ReCreate(Device device, USize size)
    {
        if (_swapChain == null && size == USize.Empty) return;
        
        foreach (var backBuffer in _backBuffers.Span)
        {
            backBuffer.Dispose();
        }
        
        _rtvHeap?.Dispose();
        
        SwapChainDescription1 scInfo = _swapChainInfo;
        scInfo.Width = size.Width;
        scInfo.Height = size.Height;
        _swapChain.ResizeBuffers(scInfo.BufferCount, scInfo.Width, scInfo.Height, scInfo.Format, scInfo.Flags).CheckError();
        
        var heapAndBackBuffer = device.CreateRenderTargetViews(BufferCount, i => _swapChain.GetBuffer<ID3D12Resource>((uint)i));
        _rtvHeap = heapAndBackBuffer.heap;
        _backBuffers = heapAndBackBuffer.resources;
    }


    public void Dispose()
    {
        _swapChain?.Dispose();
        _rtvHeap?.Dispose();

        GC.SuppressFinalize(this);
    }
}