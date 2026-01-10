using System.Numerics;
using Triangles.Module;
using Triangles.Renderer.Backend;
using Triangles.Renderer.Backend.Direct3D11;

namespace Triangles.Renderer;

public abstract class DrawElement
{
    protected IBuffer<Vertex> _vBuffer;
    protected IBuffer<uint> _iBuffer;
    protected IBuffer<Matrix4x4> _cBuffer;
    
    protected bool IsInit = false;
    protected uint IndexCount = 0;
    
    public IProgram Program;
    public TextureDx11? Texture;
    public readonly Transform Transform = new();

    public virtual void Render(DeviceDx11 device)
    {
        var context = device.Command;
        
        device.RenderPipeLine.Bind(context);
        _cBuffer.Update(context, [Transform.WorldMat], 1);
        
        Program.Bind(context);
        // Program.ConstantBuffer[1].Update(context, [Transform.WorldMat]);
        Texture?.Bind(context);
        
        _vBuffer.Bind(context);
        _iBuffer.Bind(context);
        
        context.DrawIndexed(IndexCount, 0, 0);
    }
}