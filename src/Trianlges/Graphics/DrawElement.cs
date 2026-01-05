using System.Numerics;
using Trianlges.Graphics.Direct3D11;
using Trianlges.Module;
using Trianlges.Renderer.Backend.Direct3D11;
using Vortice.Direct3D;

namespace Trianlges.Graphics;

public abstract class DrawElement
{
    protected BufferDx11<Vertex> _vBuffer;
    protected BufferDx11<uint> _iBuffer;
    protected BufferDx11<Matrix4x4> _cBuffer;
    
    protected bool IsInit = false;
    protected uint IndexCount = 0;
    
    public Material? Material { get; set; }
    public readonly Transform Transform = new ();

    public virtual void Render(Device3D device)
    {
        var context = device.DContext;

        Material ??= Material.Create(device)
            .SetShader("Assets/Default.hlsl",
                VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3, VertextType.Uv))
            .ConfigRasterizer(false, true)
            .Build<Material>();
        
        _cBuffer.Updata(context, [Transform.WorldMat], 1);
        
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        
        Material.Bind(context, 0);
        
        _vBuffer.Bind(context);
        _iBuffer.Bind(context);
        
        context.DrawIndexed(IndexCount, 0, 0);
    }
}