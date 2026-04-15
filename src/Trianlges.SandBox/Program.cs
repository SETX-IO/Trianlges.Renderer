using System;
using System.Numerics;
using Vortice.Direct3D12;
using Vortice.Mathematics;
using Triangles.BaseApp;
using Triangles.Core.RenderBg;
using Triangles.Core.Renderer;
using Vortice.Direct3D;

namespace Trianlges.SandBox;

public struct Vertex(Vector3 pos, Vector2 uv)
{
    public Vector3 Pos = pos;
    public Vector2 Uv = uv;
}

public struct ConstantData(Matrix4x4 mvp)
{
    public Matrix4x4 Mvp = mvp;

    public ConstantData() : this(Matrix4x4.Identity)
    {
    }
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
        using SwapChain swapChain = new(device, hwnd, window.Size);
        
        Fence fence = new Fence(device, swapChain.BufferCount);
        CommandBufferPool cmdPool = device.CreateCommandBufferPool(swapChain.BufferCount);
        
         Vertex[] vertices =
         [
             new (new Vector3(-0.5f, 0.5f, 0.0f), Vector2.UnitY),
             new (new Vector3(0.5f, 0.5f, 0.0f), Vector2.One),
             new (new Vector3(0.5f, -0.5f, 0.0f), Vector2.UnitX),
             new (new Vector3(-0.5f, -0.5f, 0.0f), Vector2.Zero),
         ];

         uint[] indeces = [0, 1, 2, 2, 3, 0];
        
         Buffer<Vertex> vBuffer = new(device, 4, ResourceStates.VertexAndConstantBuffer);
         Buffer<uint> iBuffer = new(device, (uint)indeces.Length, ResourceStates.IndexBuffer);
         Buffer<ConstantData> cBuffer = new Buffer<ConstantData>(device, 1, ResourceStates.GenericRead);
         
         vBuffer.SetData(vertices);
         iBuffer.SetData(indeces);
         
         cBuffer.ToConstantBuffer(ShaderVisibility.Vertex);

         Texture texture = new(device, 256, 256);
         texture.CreateView(cmdPool);
         fence.Wait();
         
         device.AddSamplerDesc()
             .SerializedRootSignature();
         
         IPipeLine defaultPipeLine = new PipeLine(device);
         
        Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(float.DegreesToRadians(45), (float)window.Size.Width / window.Size.Height, 1,
            100);
        Matrix4x4 view = Matrix4x4.CreateLookAtLeftHanded(new Vector3(0, 0, -3), Vector3.Zero, Vector3.UnitY);
         
        window.SizeChange += (obj, size) =>
        {
            fence.Wait();
            
            swapChain.ReCreate(device, size);
            
            proj = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(float.DegreesToRadians(45), (float)size.Width / size.Height, 1,
                100);
        };

        ConstantData constantData = new();
        float i = 0;
        
        app.Renderer += (renderTime) =>
        {
            ICommandBuffer cmd = cmdPool.Get("Renderer Command");
            RenderTargetTexture backBuffer = swapChain.CurrentBackBuffer;

            i += 0.015f;
            var axisX = Matrix4x4.CreateTranslation(0, MathF.Cos(i), 0);
            constantData.Mvp = Matrix4x4.Transpose(axisX * view * proj);
            
            Span<ConstantData> mapSpan = cBuffer.GetMapSpan();
            mapSpan[0] = constantData;

            cmd.ApplyPipeLine(defaultPipeLine);
            device.DescriptionHeapPool.BindDescHeap(cmd);
            
            cmd.SetViewProt(window.Size);
            cmd.SetScissor(window.Size);
            
            cmd.SetTarget(backBuffer);
            
            cmd.ClearTarget(Colors.SkyBlue, PrimitiveTopology.TriangleList);

            cmd.SetBuffer([vBuffer.VertexBufferView], indexBufferView: iBuffer.IndexBufferView);
            cmd.Draw();
            
            device.ExecuteCommandBuffer(cmd);
            cmdPool.Release(cmd);
            
            swapChain.Present(true);
            fence.Wait(true);
        };
            
        app.Run();
    }
}