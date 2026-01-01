using System.Runtime.CompilerServices;
using Trianlges.Render.Graphics.Direct3D11;
using Trianlges.Render.Module;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public abstract class DrawElement
{
    public ID3D11Buffer VertextBuffer { get; protected set; } = null!;
    public ID3D11Buffer? IndexBuffer { get; protected set; }
    public Material? Material { get; set; }
    public readonly Transfome Transfome = new ();
    
    protected ID3D11Buffer _contextBuffer;

    protected uint IndexCount = 0;
    
    public void Init(ID3D11Buffer vertexBffer, ID3D11Buffer indexBffer)
    {
        VertextBuffer = vertexBffer;
        IndexBuffer = indexBffer;
    }

    public virtual unsafe void Render(IDevice3D device)
    {
        var context = device.DContext;
        var stride = Vertex.Size;
        uint offset = 0;

        Material ??= Material.Create(device)
            .SetShader("Assets/Default.hlsl",
                VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3, VertextType.Uv))
            .ConfigRasterizer(false, true)
            .Build<Material>();
        
        context.VSSetConstantBuffers(1, [_contextBuffer]);
        
        var map = context.Map(_contextBuffer, MapMode.WriteDiscard);
        var worldMat = Transfome.WorldMat;
        Unsafe.Copy(map.DataPointer.ToPointer(), ref worldMat);
        context.Unmap(_contextBuffer);

        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        
        Material.Bind(context, 0);
        context.IASetVertexBuffers(0, 1, [VertextBuffer], [stride], [offset]);
        context.IASetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
        context.DrawIndexed(IndexCount, 0, 0);
    }
}