using System.Numerics;
using System.Threading;
using Vortice.Direct3D12;
using Vortice.Mathematics;
using Triangles.BaseApp;
using Triangles.Core.RenderBg;
using Vortice.Direct3D;

namespace Trianlges.SandBox;

public struct Vertex(Vector3 pos, Color3 color)
{
    public Vector3 Pos = pos;
    public Color3 Color = color;
}
    
public static class Program
{
    public static void Main(string[] args)
    {
        AppConfig config = new("Sandbox", resizable: true);
        Application app = new(config);

        Window window = app.MainWindow;
        nint hwnd = new WindowsHwndPoint(window).Handel;

        Device device = new();
        using SwapChain swapChain = new();
        swapChain.Create(device, hwnd, window.Size);
        
        CommandBufferPool cmdPool = device.CreateCommandBufferPool(swapChain.BufferCount);
        
         Vertex[] vertices =
         [
             new (new Vector3(-0.5f, 0.5f, 0.0f), new Color3(1f, 0, 0)),
             new (new Vector3(0.5f, 0.5f, 0.0f), new Color3(0, 1f, 0)),
             new (new Vector3(0.5f, -0.5f, 0.0f), new Color3(0, 0, 1f)),
             new (new Vector3(-0.5f, -0.5f, 0.0f), new Color3(0, 0 ,0)),
         ];

         uint[] indeces = [0, 1, 2, 2, 3, 0];
         
         // Vertex[] vertices =
         // [
         //     new (new Vector3(0f, 0.5f, 0.0f), new Color3(1f, 0, 0)),
         //     new (new Vector3(0.5f, -0.5f, 0.0f), new Color3(0, 1f, 0)),
         //     new (new Vector3(-0.5f, -0.5f, 0.0f), new Color3(0, 0 ,1f)),
         // ];

         PipeLine defaultPipeLine = new(device);
         Buffer<Vertex> vBuffer = new(device, 4, ResourceStates.VertexAndConstantBuffer);
         Buffer<uint> iBuffer = new(device, (uint)indeces.Length, ResourceStates.IndexBuffer);
         vBuffer.SetData(vertices);
         iBuffer.SetData(indeces);

         Fence fence = new Fence(device, swapChain.BufferCount);

         window.SizeChange += (obj, size) =>
        {
            fence.Wait();
            
            swapChain.ReCreate(device, size);
        };
            
        app.Renderer += (renderTime) =>
        {
            CpuDescriptorHandle rtbHandle = swapChain.CurrentRtv;
            CommandBuffer cmd = cmdPool.Get("Renderer Command");
            ID3D12Resource backBuffer = swapChain.CurrentBackBuffer;
            
            cmd.ApplyPipeLine(defaultPipeLine);
            
            cmd.SetViewProt(window.Size);
            cmd.SetScissor(window.Size);
            
            cmd.ResourceBarrier(backBuffer);
            
            cmd.SetTarget(rtbHandle);
            cmd.ClearTarget(new Color4(0, 0.2f, 0.4f), PrimitiveTopology.TriangleList);

            cmd.SetBuffer([vBuffer.VertexBufferView], indexBufferView: iBuffer.IndexBufferView);
            cmd.Draw();
            
            cmd.ResourceBarrier(backBuffer);
            
            device.ExecuteCommandBuffer(cmd);
            cmdPool.Release(cmd);
            
            swapChain.Present();
            
            fence.Wait(true);
        };
            
        app.Run();
    }
}