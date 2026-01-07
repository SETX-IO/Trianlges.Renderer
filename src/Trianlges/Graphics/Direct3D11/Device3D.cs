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
    private readonly IDXGIFactory1 _factory;
    private Viewport _viewport;
    private ID3D11Texture2D _depthStencil;

    /// <summary>
    ///     Default constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Device3D device = new Device3D();
    /// device.CreateWindowResource(windowHandler)
    /// </code>
    public Device3D()
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

        try
        {
            if (!result.Success)
                throw new COMException();
        }
        catch (COMException)
        {
            Console.WriteLine("[Wirring] You can't enable debug layer because you haver't installed the DirectX debugger Tools.");
            createFlags ^= DeviceCreationFlags.Debug;
            
            result = D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, createFlags, [], out device, out _, out context);
            if (!result.Success) throw;
        }
        
        Device = device;
        DContext = context;

        RenderPipeLine = new RenderPipeLineDx11(device);
    }

    /// <summary>
    ///     Create constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// Device3D device = new Device3D();
    /// device.CreateWindowResource(windowHandler)
    /// </code>
    /// <param name="windowHandler">Win32 HWND</param>
    public Device3D(IntPtr windowHandler) : this()
    {
        // Create(windowHandler);
        // ConfigRenderTarget();
        CreateWindowResource(windowHandler);
    }

    public ID3D11Device Device { get; }
    public ID3D11DeviceContext DContext { get; }

    public RenderPipeLineDx11 RenderPipeLine { get; protected set; }

    public IDXGISwapChain SwapChain { get; private set; } = null!;
    public ID3D11RenderTargetView? RenderTarget { get; private set; }
    public ID3D11DepthStencilView? DepthStencil { get; private set; }

    public void CreateWindowResource(IntPtr windowHandler)
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
    
    public void ResetSize(uint width, uint height)
    {
        if (width == 0 || height == 0) return;
        
        var res = SwapChain.ResizeBuffers(1, width, height, Format.B8G8R8A8_UNorm);
        
        if (!res.Success)
            return;
        
        RenderTarget?.Release();
        DepthStencil?.Release();
        
        ConfigRenderTarget();
    }

    public void ConfigRenderTarget()
    {
        ID3D11Texture2D backBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
        RenderTarget = Device.CreateRenderTargetView(backBuffer);
        
        Texture2DDescription bbDesc = backBuffer.Description;
        
        Texture2DDescription depthDesc = new(Format.D24_UNorm_S8_UInt, bbDesc.Width, bbDesc.Height, 1, 1, BindFlags.DepthStencil);
        _depthStencil = Device.CreateTexture2D(depthDesc);
        
        var depthViewDesc = new DepthStencilViewDescription(DepthStencilViewDimension.Texture2D);
        DepthStencil = Device.CreateDepthStencilView(_depthStencil, depthViewDesc);
        DContext.OMSetRenderTargets([RenderTarget], DepthStencil);

        _viewport = new Viewport(bbDesc.Width, bbDesc.Height);
        
        DContext.RSSetViewports([_viewport]);
    }

    public BufferDx11<T> NewBuffer<T>(BindFlags bufferType, T[]? data = null, uint bufferSize = 0, bool isDynamic = false) where T : unmanaged
    {
        BufferDx11<T> buffer;
        if (bufferSize != 0)
        {
            buffer = new BufferDx11<T>(Device, bufferType, bufferSize);
            return buffer;
        }
        
        buffer = new BufferDx11<T>(Device, bufferType, data, isDynamic);
        return buffer;
    }

    public TextureDx11 NewTexture(string textureName)
    {
        var texture =  new TextureDx11(Device, textureName);
        return texture;
    }

    public RenderPipeLineDx11 NewRenderPipeLine()
    {
        RenderPipeLineDx11 renderPipeLine = new RenderPipeLineDx11(Device);
        return renderPipeLine;
    }

    public void Clear()
    {
        DContext.ClearRenderTargetView(RenderTarget, Camera.ClearColor);
        DContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
    }
    
    public void Present() => SwapChain.Present(0, PresentFlags.None);
}