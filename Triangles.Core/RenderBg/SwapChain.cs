using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class SwapChain : IDisposable
{
    private IDXGISwapChain4? _swapChain;
    
    private ReadOnlyMemory<RenderTargetTexture> _backBuffers;
    private SwapChainDescription1 _swapChainInfo;

    public RenderTargetTexture CurrentBackBuffer => _backBuffers.Span[(int)CurrentIndex];
    public uint CurrentIndex => _swapChain.CurrentBackBufferIndex;
    public uint BufferCount => _swapChainInfo.BufferCount;

    public SwapChain(Device device, nint windowHandel, USize windowSize)
    {
        _swapChainInfo = new SwapChainDescription1(windowSize.Width, windowSize.Height, Format.R8G8B8A8_UNorm);

        using IDXGISwapChain1 swapChain =
            Dxgi.Factory.Value.CreateSwapChainForHwnd(device.CommandQueue, windowHandel, _swapChainInfo);
        
        _swapChain = swapChain.QueryInterface<IDXGISwapChain4>();
        
        _backBuffers = device.CreateRenderTargetViews(BufferCount, i => _swapChain.GetBuffer<ID3D12Resource>((uint)i));
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
        
        SwapChainDescription1 scInfo = _swapChainInfo;
        scInfo.Width = size.Width;
        scInfo.Height = size.Height;
        _swapChain.ResizeBuffers(scInfo.BufferCount, scInfo.Width, scInfo.Height, scInfo.Format, scInfo.Flags).CheckError();
        
        _backBuffers = device.CreateRenderTargetViews(BufferCount, i => _swapChain.GetBuffer<ID3D12Resource>((uint)i));
    }


    public void Dispose()
    {
        _swapChain?.Dispose();

        GC.SuppressFinalize(this);
    }
}