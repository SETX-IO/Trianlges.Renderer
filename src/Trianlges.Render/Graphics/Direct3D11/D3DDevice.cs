using System;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

/// <summary>
///     Marager DirectX11 Device.
/// </summary>
public class D3DDevice : IDevice3D
{
    private Viewport _viewport;

    /// <summary>
    ///     Default constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Create(windowHandler);
    /// ConfigRenderTarget();
    /// </code>
    public D3DDevice()
    {
    }

    /// <summary>
    ///     Create constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Create(windowHandler);
    /// ConfigRenderTarget();
    /// </code>
    /// <param name="windowHandler">Win32 HWND</param>
    public D3DDevice(IntPtr windowHandler)
    {
        Create(windowHandler);
        ConfigRenderTarget();
    }

    public ID3D11Device Device { get; private set; } = null!;
    public ID3D11DeviceContext DContext { get; private set; } = null!;

    public IDXGISwapChain SwapChain { get; private set; } = null!;
    public ID3D11RenderTargetView? RenderTarget { get; private set; }
    public ID3D11DepthStencilView? DepthStencil { get; private set; }

    public void Create(IntPtr windowHandler)
    {
        var swDesc = new SwapChainDescription
        {
            BufferCount = 1,
            BufferDescription = new ModeDescription
            {
                Format = Format.R8G8B8A8_UNorm
            },
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = windowHandler,
            SampleDescription = new SampleDescription(1, 0),
            Windowed = true
        };

        var createFlags = DeviceCreationFlags.None;

#if DEBUG
        createFlags |= DeviceCreationFlags.Debug;
#endif
        
        D3D11.D3D11CreateDeviceAndSwapChain(
            null, DriverType.Hardware,
            createFlags, null!,
            swDesc, out var sw,
            out var device, out _,
            out var context).CheckError();

        if (device == null && context == null && sw == null)
            throw new COMException();

        Device = device!;
        DContext = context!;
        SwapChain = sw!;
    }
    
    public void ResetSize(uint width, uint hieght)
    {
        if (width == 0 || hieght == 0) return;
        
        SwapChain.ResizeBuffers(1, width, hieght);
        RenderTarget?.Release();
        DepthStencil?.Release();
        
        ConfigRenderTarget();
    }

    public void ConfigRenderTarget()
    {
        var backBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
        RenderTarget = Device.CreateRenderTargetView(backBuffer);
        var bbDesc = backBuffer.Description;

        var depthDesc = new Texture2DDescription(Format.D24_UNorm_S8_UInt, bbDesc.Width, bbDesc.Height, 1, 1,
            BindFlags.DepthStencil);
        var depthViewDesc = new DepthStencilViewDescription(DepthStencilViewDimension.Texture2D);

        var depthBuffer = Device.CreateTexture2D(depthDesc);
        DepthStencil = Device.CreateDepthStencilView(depthBuffer, depthViewDesc);

        DContext.OMSetRenderTargets([RenderTarget], DepthStencil);

        _viewport = new Viewport(0, 0, bbDesc.Width, bbDesc.Height, 0, 1);
        
        DContext.RSSetViewports([_viewport]);
    }

    void IDevice.Present()
    {
        SwapChain.Present(0, PresentFlags.None);
    }
}