using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Trianlges.Graphics.Direct2D;
using Trianlges.Renderer.Backend.Direct3D11;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Trianlges.Graphics.Direct3D11;

/// <summary>
///     Marager DirectX11 Device.
/// </summary>
public class Device3D : IDevice3D, IDevice2D
{
    // private readonly Lazy<Device3D> _instance = new Lazy<Device3D>(new Device3D());
    // public Device3D Instance => _instance.Value;
    
    private Viewport _viewport;
    private IDXGIFactory1 _factory;

    /// <summary>
    ///     Default constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Create(windowHandler);
    /// ConfigRenderTarget();
    /// </code>
    public Device3D()
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
    public Device3D(IntPtr windowHandler)
    {
        // Create(windowHandler);
        // ConfigRenderTarget();
        Create();
        CreateWindowResouce(windowHandler);
    }

    public ID3D11Device Device { get; protected set; } = null!;
    public ID3D11DeviceContext DContext { get; private set; } = null!;

    public IDXGISwapChain SwapChain { get; private set; } = null!;
    public ID3D11RenderTargetView? RenderTarget { get; private set; }
    public ID3D11DepthStencilView? DepthStencil { get; private set; }

    public void Create()
    {
        Result result = Result.Ok;
        
        var createFlags = DeviceCreationFlags.BgraSupport;
        
#if DEBUG
        createFlags |= DeviceCreationFlags.Debug;
#endif
        DXGI.CreateDXGIFactory1<IDXGIFactory6>(out var factory);
        factory!.EnumAdapterByGpuPreference(0, GpuPreference.HighPerformance, out IDXGIAdapter? adapter);

        _factory = factory;
        
        result = D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, createFlags, [], out var device, out _, out var context);
        
        if (!result.Success)
            throw new COMException();

        Device = device;
        DContext = context;
    }

    public void CreateWindowResouce(IntPtr windowHandler)
    {
        var swDesc = new SwapChainDescription
        {
            BufferCount = 1,
            BufferDescription = new ModeDescription
            {
                Format = Format.B8G8R8A8_UNorm
            },
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = windowHandler,
            SampleDescription = new SampleDescription(1, 0),
            Windowed = true
        };
        
        SwapChain = _factory.CreateSwapChain(Device, swDesc);
        
        ConfigRenderTarget();
    }
    
    public void ResetSize(uint width, uint hieght)
    {
        if (width == 0 || hieght == 0) return;
        
        var res = SwapChain.ResizeBuffers(1, width, hieght, Format.B8G8R8A8_UNorm);
        
        if (!res.Success)
            return;
        
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

        _viewport = new Viewport(bbDesc.Width, bbDesc.Height);
        
        DContext.RSSetViewports([_viewport]);
    }

    public BufferDx11<T> NewBuffer<T>(BindFlags bufferType, T[]? data = null, bool isDyamic = false) where T : unmanaged
    {
        var buffer = new BufferDx11<T>(Device, bufferType, data, isDyamic);
        return buffer;
    }

    public void Clear()
    {
        DContext.ClearRenderTargetView(RenderTarget, Camera.ClearColor);
        DContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
    }
    
    public void Present() => SwapChain.Present(0, PresentFlags.None);
}