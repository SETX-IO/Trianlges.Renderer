using System.Numerics;
using Trianlges.Render.Graphics.Direct3D11;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public abstract class DrawElement
{
    public ID3D11Buffer VertextBuffer { get; protected set; } = null!;
    public ID3D11Buffer? IndexBuffer { get; protected set; }
    public ShaderProgame? Progame { get; protected set; }

    public void Init(IDevice3D device3D)
    {
        var device = device3D.Device;
        
        var vertextDesc = VertexInputElement.GetVertextElements(VertextType.Positon, VertextType.Color3);

        Progame = ShaderProgame
            .Create(device)
            .Complier("Assets/Shader.hlsl")
            .ConfigInput(vertextDesc)
            .Build();
    }
    
    public virtual void Updata(IDevice3D device)
    {
    }

    public virtual void Render(IDevice3D device)
    {
        var context = device.DContext;
        var stride = Vertex.Size;
        uint offset = 0;

        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

        Progame.Bind(context);
        
        context.IASetVertexBuffers(0, 1, [VertextBuffer], [stride], [offset]);
        
        if (IndexBuffer != null) context.IASetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
    }
}