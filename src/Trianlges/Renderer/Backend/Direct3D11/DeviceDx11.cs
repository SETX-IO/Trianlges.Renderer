using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class DeviceDx11 : IDevice
{
    private IDXGIFactory1 _factory = null!;
    private ID3D11Device _device = null!;
    private ID3D11DeviceContext _context = null!;
    private IDXGISwapChain _swapchain = null!;
    private static Lazy<DeviceDx11> _instance = new(new DeviceDx11());
    
    public static IDevice Instance => _instance.Value;

    private DeviceDx11()
    {
        Create();
    }
    
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

        _device = device;
        _context = context;
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
        
        _swapchain = _factory.CreateSwapChain(_device, swDesc);
    }
}