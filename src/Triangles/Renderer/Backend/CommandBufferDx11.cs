using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace Triangles.Renderer.Backend;

public class CommandBufferDx11
{
    protected ID3D11DeviceContext Context;
    public Viewport Viewport;
    public ID3D11RenderTargetView RenderTarget;
    public ID3D11DepthStencilView? DepthStencil;
    
    public IProgram Program;

    public CommandBufferDx11(ID3D11DeviceContext context)
    {
        Context = context;
        context.AddRef();
    }
    
    public void SetViewPort(Viewport viewport)
    {
        Viewport = viewport;
        SetViewPort(viewport.Width, viewport.Height, viewport.X, viewport.Y, viewport.MaxDepth, viewport.MaxDepth);
    }
    
    public void SetViewPort(float width, float height, float x = 0, float y = 0, float miniDepth = 0f, float maxDepth = 1f)
    {
        Viewport viewport = new Viewport(x, y, width, height, miniDepth, maxDepth);
        Viewport = viewport;
        
        Context.RSSetViewports([Viewport]);
    }

    public void SetRenderTarget(ID3D11RenderTargetView renderTarget, ID3D11DepthStencilView? depthStencil = null)
    {
        RenderTarget = renderTarget;
        DepthStencil = depthStencil;
        Context.OMSetRenderTargets([RenderTarget], DepthStencil);
    }

    public void SetBuffer<TVertex, TIndex>(IBuffer<TVertex> vertexBuffer, IBuffer<TIndex> indexBuffer)
        where TVertex : unmanaged
        where TIndex: unmanaged
    {
        
        
        vertexBuffer.Bind(Context);
        indexBuffer.Bind(Context);
    }
    
    public void SetProgram(IProgram program)
    {
        Program = program;
        Program.Bind(Context);
    }

    public void Release()
    {
        Context.Release();
    }
}