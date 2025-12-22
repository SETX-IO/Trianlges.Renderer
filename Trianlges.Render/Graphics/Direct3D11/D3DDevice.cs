using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

/// <summary>
/// Marager DirectX11 Device.
/// </summary>
public class D3DDevice
{
    private ID3D11Texture2D? _backBuffer;
    private ID3D11Texture2D? _depthBuffer;
    private Viewport _viewport;
    
    public ID3D11Device Device { get; private set; }
    public ID3D11DeviceContext DContext { get; private set; }
    
    public IDXGISwapChain SwapChain { get; private set; }
    public ID3D11RenderTargetView? RenderTarget { get; private set; }
    public ID3D11DepthStencilView? DepthStencil { get; private set; }

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

        if (device == null && context == null && sw == null)
            throw new COMException();
        
        Device = device!;
        DContext = context!;
        SwapChain = sw!;
    }

    public void CreateRenderResouce()
    {
         _backBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
         RenderTarget = Device.CreateRenderTargetView(_backBuffer);
         var bbDesc = _backBuffer.Description;

         var depthDesc = new Texture2DDescription(Format.D24_UNorm_S8_UInt, bbDesc.Width, bbDesc.Height, 1, 1, BindFlags.DepthStencil);
         var depthViewDesc = new DepthStencilViewDescription(DepthStencilViewDimension.Texture2D);

         _depthBuffer = Device.CreateTexture2D(depthDesc);
         DepthStencil = Device.CreateDepthStencilView(_depthBuffer, depthViewDesc);
         
         _viewport = new Viewport
         {
            Width = bbDesc.Width,
            Height = bbDesc.Height,
            MaxDepth = 1f,
            MinDepth = 0f
         };
         
         DContext.OMSetRenderTargets([RenderTarget]);
         DContext.RSSetViewports([_viewport]);
    }

    public void Present()
    {
        SwapChain.Present(0, PresentFlags.None);
    }
    
    public void Release()
    {
        Device.Release();
        DContext.Release();
    }
}