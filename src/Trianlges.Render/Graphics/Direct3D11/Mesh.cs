using System;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            new Vertex(new Vector3(-0.5f, 0.5f, 0), Vector2.Zero),
            new Vertex(new Vector3(0.5f, 0.5f, 0), Vector2.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, 0), Vector2.One),
            new Vertex(new Vector3(-0.5f, -0.5f, 0),  Vector2.UnitY)
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
        IndexCount = (uint)indiecs.Length;
    }

    private void CreateRenderResouces(ID3D11Device device)
    {
        var vBufferDesc =
            new BufferDescription((uint)(_vertices.Length * Vertex.Size), BindFlags.VertexBuffer,
                ResourceUsage.Immutable);

        var vData = DataStream.Create(_vertices, true, true);
        VertextBuffer = device.CreateBuffer(vBufferDesc, vData);

        var cBufferDesc = new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BindFlags.ConstantBuffer,
            ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _contextBuffer = device.CreateBuffer(cBufferDesc);
        
        var iBufferDesc =
            new BufferDescription((uint)(_indiecs.Length * sizeof(uint)), BindFlags.IndexBuffer,
                ResourceUsage.Immutable);

        var iData = DataStream.Create(_indiecs, true, true);
        IndexBuffer = device.CreateBuffer(iBufferDesc, iData);
    }

    public override void Render(IDevice3D device)
    {
        if (IndexBuffer == null)
        {
            CreateRenderResouces(device.Device);
        }

        base.Render(device);
    }
}