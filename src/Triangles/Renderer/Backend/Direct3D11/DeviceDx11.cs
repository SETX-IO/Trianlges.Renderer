using System;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Triangles.Renderer.Backend.Direct3D11;

/// <summary>
///     Marager DirectX11 Device.
/// </summary>
public class DeviceDx11
{
    /// <summary>
    ///     Default constructors.
    /// </summary>
    /// <code>
    /// // You or use Code.
    /// // windowHandler is win32 HWND.
    /// DeviceDx11 device = new DeviceDx11();
    /// device.CreateWindowResource(windowHandler)
    /// </code>
    public DeviceDx11()
    {
        Result result;
        
        var createFlags = DeviceCreationFlags.BgraSupport;
        
#if DEBUG
        createFlags |= DeviceCreationFlags.Debug;
#endif
        IDXGIFactory6 factory = UtiltDx.GetDxgiIFactory<IDXGIFactory6>();
        factory.EnumAdapterByGpuPreference(0, GpuPreference.HighPerformance, out IDXGIAdapter? adapter);
        
        result = D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, createFlags, [], out var device, out _, out var context);

        try
        {
            if (!result.Success)
                throw new COMException("[Warning] You can't enable debug layer because you haver't installed the DirectX debugger Tools.");
        }
        catch (COMException e)
        {
            Console.WriteLine(e.Message);
            
            createFlags ^= DeviceCreationFlags.Debug;
            
            result = D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, createFlags, [], out device, out _, out context);
            if (!result.Success) throw;
        }
        
        Device = device;
        DContext = context;

        CommandPool.Init(Device);
        
        RenderPipeLine = new RenderPipeLineDx11(device);
        Command = Device.CreateDeferredContext();
    }

    public ID3D11Device Device { get; }
    public ID3D11DeviceContext DContext { get; }
    public ID3D11DeviceContext Command;

    public RenderPipeLineDx11 RenderPipeLine { get; protected set; }

    public BufferDx11<T> NewBuffer<T>(BindFlags bufferType, T[]? data = null, uint bufferSize = 0) where T : unmanaged
    {
        BufferDx11<T> buffer;
        if (bufferSize != 0)
        {
            buffer = new BufferDx11<T>(Device, bufferType, bufferSize);
            return buffer;
        }
        
        buffer = new BufferDx11<T>(Device, bufferType, data);
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

    public ProgramDx11 NewProgram(string vShaderPath, string pShaderPath)
    {
        ProgramDx11 program = new ProgramDx11(Device, vShaderPath, pShaderPath);
        return program;
    }
    
    public void SubmitCommandBuffer(ID3D11DeviceContext context)
    {
        if (context.ContextType != DeviceContextType.Deferred)
            throw new ArgumentException($"{nameof(context)} not is deferred context.");
        
        context.FinishCommandList(false, out var commandList);
        DContext.ExecuteCommandList(commandList, true);
    } 
    
    public void Submit()
    {
        Command.FinishCommandList(false, out var list);
        DContext.ExecuteCommandList(list, true);
    } 
}