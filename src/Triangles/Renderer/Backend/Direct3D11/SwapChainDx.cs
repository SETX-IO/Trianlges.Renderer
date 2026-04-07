using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Color = Vortice.Mathematics.Color;
using Size = System.Drawing.Size;

namespace Triangles.Renderer.Backend.Direct3D11;

public static class WindowSwapChain
{
    public static SwapChainDx CreateSwapChain(this Window self, ID3D11Device device, int bufferCount = 1)
    {
        SwapChainDx swapChain = new SwapChainDx(device, self.Size, self.Win32Handler, bufferCount);
        return swapChain;
    }

    public static Viewport GetViewport(this Window self, int x = 0, int y = 0, float minDepth = 0, float maxDepth = 1f)
    {
        Viewport viewport;
        
        viewport.X = x;
        viewport.Y = y;
        
        viewport.Width = self.Size.Width;
        viewport.Height = self.Size.Height;

        viewport.MinDepth = minDepth;
        viewport.MaxDepth = maxDepth;
        
        return viewport;
    }
}

public class SwapChainDx
{
    private readonly int _bufferCount;
    private int _backBufferIndex;
    
    public Size FrameSize; 
    public readonly IDXGISwapChain SwapChain;
    public readonly ID3D11RenderTargetView[] RenderTargets;
    public ID3D11DepthStencilView? DepthStencil;
    
    public SwapChainDx(ID3D11Device device, Size frameSize, nint outWindowHwnd, int bufferCount, string debugName = "")
    {
        var swDesc = new SwapChainDescription
        {
            BufferCount = (uint)bufferCount,
            BufferDescription = new ModeDescription((uint)frameSize.Width, (uint)frameSize.Height),
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = outWindowHwnd,
            SampleDescription = new SampleDescription(1, 0),
            Windowed = true
        };

        _bufferCount = bufferCount;
        SwapChain = UtiltDx.GetDxgiIFactory<IDXGIFactory1>().CreateSwapChain(device, swDesc);
        RenderTargets = new ID3D11RenderTargetView[swDesc.BufferCount];

        ConfigBackBuffer(device);
        
        if (string.IsNullOrEmpty(debugName))
            SwapChain.DebugName = $"{debugName}_{nameof(SwapChain)}";
    }
    
    public void Present()
    {
        SwapChain.Present(0, PresentFlags.Restart);
    }
    
    public void ClearScreen(ID3D11DeviceContext context, Color clearColor)
    {
        context.OMSetRenderTargets([RenderTargets[_backBufferIndex]], DepthStencil);
        
        context.ClearRenderTargetView(RenderTargets[_backBufferIndex], clearColor);
        context.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
        
        _backBufferIndex = (_bufferCount + 1) % _bufferCount;
    }

    private void ConfigBackBuffer(ID3D11Device device)
    {
        for (uint i = 0; i < _bufferCount; i++)
        {
            var buffer = SwapChain.GetBuffer<ID3D11Texture2D>(i);
            var backBufferInfo = buffer.Description;

            if (FrameSize.IsEmpty)
                FrameSize = new Size((int)backBufferInfo.Width, (int)backBufferInfo.Height);
            
            RenderTargets[i] = device.CreateRenderTargetView(buffer);
        }
        
        Texture2DDescription depthDesc = new(Format.D24_UNorm_S8_UInt, (uint)FrameSize.Width, (uint)FrameSize.Height, 1, 1, BindFlags.DepthStencil);
        var depthStencil = device.CreateTexture2D(depthDesc);
        
        var depthViewDesc = new DepthStencilViewDescription(DepthStencilViewDimension.Texture2D);
        DepthStencil = device.CreateDepthStencilView(depthStencil, depthViewDesc);
    }
}