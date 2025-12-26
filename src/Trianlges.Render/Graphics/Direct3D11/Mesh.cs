using System;
using System.Numerics;
using Vortice;
using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Mesh : DrawElement
{
    public static readonly Mesh Trianlge;
    public static readonly Mesh Quadrilateral;
    public static readonly Mesh Cube;
    
    private uint[] _indiecs = [];
    private Vertex[] _vertices = null!;

    public Mesh()
    {
        
    }
    
    static Mesh()
    {
        Trianlge = new Mesh();
        Quadrilateral = new Mesh();
        Cube = new Mesh();

        Vertex[] vertices =
        [
            new(new Vector3(0, 0.5f, 0), Vector3.UnitX),
            new(new Vector3(0.5f, -0.5f, 0), Vector3.UnitY),
            new(new Vector3(-0.5f, -0.5f, 0), Vector3.UnitZ)
        ];

        uint[] indiecs = [0,1,2];
        
        Trianlge.Init(vertices, indiecs);

        vertices =
        [
            new Vertex(new Vector3(-0.5f, 0.5f, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, 0.5f, 0), Vector3.UnitY),
            new Vertex(new Vector3(0.5f, -0.5f, 0), Vector3.UnitZ),
            new Vertex(new Vector3(-0.5f, -0.5f, 0), new Vector3(1,1,0))
        ];

        indiecs = [0,1,2,   2,3,0];
        
        Quadrilateral.Init(vertices, indiecs);
        
        vertices =
        [
            new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, 0.0f)),
            new Vertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new Vertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 0.0f)),
            new Vertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new Vertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new Vertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f)),
            new Vertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f)),
            new Vertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f))
        ];

        indiecs =
        [
            // 正面
            0, 1, 2,    2, 3, 0,
            // 左面
            4, 5, 1,    1, 0, 4,
            // 顶面
            1, 5, 6,    6, 2, 1,
            // 背面
            7, 6, 5,    5, 4, 7,
            // 右面
            3, 2, 6,    6, 7, 3,
            // 底面
            4, 0, 3,    3, 7, 4
        ];

        Cube.Init(vertices, indiecs);
    }

    private void Init(Vertex[] vertices, uint[] indiecs)
    {
        _vertices = vertices;
        _indiecs = indiecs;
    }

    private void CreateRenderResouces(ID3D11Device device)
    {
        var vBufferDesc =
            new BufferDescription((uint)(_vertices.Length * Vertex.Size), BindFlags.VertexBuffer,
                ResourceUsage.Immutable);

        var vData = DataStream.Create(_vertices, true, true);
        VertextBuffer = device.CreateBuffer(vBufferDesc, vData);

        if (_indiecs is { Length: 0 } && IndexBuffer != null) return;
        
        var iBufferDesc =
            new BufferDescription((uint)(_indiecs.Length * sizeof(uint)), BindFlags.IndexBuffer,
                ResourceUsage.Immutable);

        var iData = DataStream.Create(_indiecs, true, true);
        IndexBuffer = device.CreateBuffer(iBufferDesc, iData);
    }

    public override void Render(IDevice3D device)
    {
        var context = device.DContext;

        if (IndexBuffer == null)
        {
            var vertextDesc = VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Color3);
            
            Progame = ShaderProgame
                .Create(device.Device)
                .Complier("Assets/Shader")
                .ConfigInput(vertextDesc)
                .Build();
            
            CreateRenderResouces(device.Device);
        }

        base.Render(device);
        
        if (IndexBuffer == null)
            context.Draw((uint)_vertices.Length, 0);
        else
            context.DrawIndexed((uint)_indiecs.Length, 0, 0);
    }
}