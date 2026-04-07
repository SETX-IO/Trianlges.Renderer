using System;
using Microsoft.VisualBasic.CompilerServices;
using Vortice.Direct3D11;

namespace Triangles.Renderer.Backend.Direct3D11;

public class CommandPool
{
    private ID3D11DeviceContext _deferredContext;
    private static CommandPool Instance;
    private static bool init;

    public static void Init(ID3D11Device device)
    {
        if (init)
            return;

        Instance = new CommandPool();
        
        Instance._deferredContext = device.CreateDeferredContext();
        
        init = true;
    }

    public static CommandBufferDx11 Get()
    {
        if (!init)
            throw new NullReferenceException("CommandPool not init.");
            
        return Instance.GetCommandBuffer();
    }

    public static void Release(CommandBufferDx11 cmdBuffer)
    {
        if (!init)
            throw new NullReferenceException("CommandPool not init.");
        
        Instance.ReleaseBuffer(cmdBuffer);
    }

    public CommandBufferDx11 GetCommandBuffer()
    {
        CommandBufferDx11 buffer = new (_deferredContext);
        return buffer;
    }

    public void ReleaseBuffer(CommandBufferDx11 cmdBuffer)
    {
        _deferredContext.ClearState();
        cmdBuffer.Release();
    }
}