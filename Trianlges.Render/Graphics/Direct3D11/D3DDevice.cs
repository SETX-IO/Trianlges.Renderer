using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

public class D3DDevice
{
    private ID3D11Texture2D? _backBuffer;
    private Viewport _viewport;
    
    public ID3D11Device? Device { get; private set; }
    public ID3D11DeviceContext? DContext { get; private set; }
    
    public IDXGISwapChain? SwapChain { get; private set; }
    public ID3D11RenderTargetView? RenderTarget { get; private set; }

    /// <summary>
    /// Default constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Create(windowHandler);
    /// CreateRenderResouce();
    /// </code>
    public D3DDevice() {}
    
    /// <summary>
    /// Create constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Create(windowHandler);
    /// CreateRenderResouce();
    /// </code>
    /// <param name="windowHandler">Win32 HWND</param>
    public D3DDevice(IntPtr windowHandler)
    {
        Create(windowHandler);
        CreateRenderResouce();
    }

    public void Create(IntPtr windowHandler)
    {
        var swDesc = new SwapChainDescription
        {
            BufferCount = 1,
            BufferDescription = new ModeDescription
            {
                Format = Format.R8G8B8A8_UNorm,
            },
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = windowHandler,
            SampleDescription = new SampleDescription
            {
                Count = 1,
                Quality = 0
            },
            Windowed = true
        };

        D3D11.D3D11CreateDeviceAndSwapChain(
            null, DriverType.Hardware,
            default, null!,
            swDesc, out var sw,
            out var device, out _,
            out var context).CheckError();

        Device = device;
        DContext = context;
        SwapChain = sw;
    }

    public void CreateRenderResouce()
    {
        if (Device == null) return;
        if (DContext == null) return;
        if (SwapChain == null) return;
        
         _backBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
         RenderTarget = Device.CreateRenderTargetView(_backBuffer);
         var bbDesc = _backBuffer.Description;
         
         _viewport = new Viewport
         {
            Width = bbDesc.Width,
            Height = bbDesc.Height
         };
         
         DContext.OMSetRenderTargets([RenderTarget]);
         DContext.RSSetViewports([_viewport]);
    }

    public void Present()
    {
        SwapChain?.Present(0, PresentFlags.None);
    }
    
    public void Release()
    {
        Device?.Release();
        DContext?.Release();
    }
}