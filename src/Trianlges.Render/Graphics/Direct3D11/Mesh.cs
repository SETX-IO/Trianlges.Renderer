using System;
using System.Numerics;
using Vortice;
using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Mesh : DrawElement
{
    // public static readonly Mesh Trianlge;
    public static readonly Mesh Quadrilateral;
    // public static readonly Mesh Cube;
    
    private uint[] _indiecs = [];
    private VertexUv[] _vertices = null!;

    public Mesh()
    {
        
    }
    
    static Mesh()
    {
        // Trianlge = new Mesh();
        Quadrilateral = new Mesh();
        // Cube = new Mesh();
        //
        // Vertex[] vertices =
        // [
        //     new(new Vector3(0, 0.5f, 0), Vector3.UnitX),
        //     new(new Vector3(0.5f, -0.5f, 0), Vector3.UnitY),
        //     new(new Vector3(-0.5f, -0.5f, 0), Vector3.UnitZ)
        // ];
        //
        // uint[] indiecs = [0,1,2];
        //
        // Trianlge.Init(vertices, indiecs);
        //
        VertexUv[] vertices =
        [
            new VertexUv(new Vector3(-0.5f, 0.5f, 0), Vector2.Zero),
            new VertexUv(new Vector3(0.5f, 0.5f, 0), Vector2.UnitX),
            new VertexUv(new Vector3(0.5f, -0.5f, 0), Vector2.One),
            new VertexUv(new Vector3(-0.5f, -0.5f, 0),  Vector2.UnitY)
        ];
        
        uint[] indiecs = [0,1,2,   2,3,0];
        
        Quadrilateral.Init(vertices, indiecs);
        //
        // vertices =
        // [
        //     new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, 0.0f)),
        //     new Vertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
        //     new Vertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 0.0f)),
        //     new Vertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
        //     new Vertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)),
        //     new Vertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f)),
        //     new Vertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f)),
        //     new Vertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f))
        // ];
        //
        // indiecs =
        // [
        //     // 正面
        //     0, 1, 2,    2, 3, 0,
        //     // 左面
        //     4, 5, 1,    1, 0, 4,
        //     // 顶面
        //     1, 5, 6,    6, 2, 1,
        //     // 背面
        //     7, 6, 5,    5, 4, 7,
        //     // 右面
        //     3, 2, 6,    6, 7, 3,
        //     // 底面
        //     4, 0, 3,    3, 7, 4
        // ];
        //
        // Cube.Init(vertices, indiecs);
    }

    private void Init(VertexUv[] vertices, uint[] indiecs)
    {
        _vertices = vertices;
        _indiecs = indiecs;
    }

    private void CreateRenderResouces(ID3D11Device device)
    {
        var vBufferDesc =
            new BufferDescription((uint)(_vertices.Length * VertexUv.Size), BindFlags.VertexBuffer,
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
            // InputElementDescription[] vertextDesc = [
            //     new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0u, 0u, InputClassification.PerVertexData, 0),
            //     new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 12u, 0u, InputClassification.PerVertexData, 0),
            // ];
            var vertextDesc = VertexInputElement.GetVertextElements(VertextType.Position, VertextType.Uv);
            
            Progame = ShaderProgame
                .Create(device.Device)
                .Complier("Assets/TextureShader.hlsl")
                .ConfigInput(vertextDesc)
                .Build<ShaderProgame>();

            var texture = Texture.Create(device.Device)
                .LoadFormFile("Assets/image.jpg")
                .Build<Texture>();
            
            CreateRenderResouces(device.Device);

            var samplerDes = new SamplerDescription
            {
                Filter = Filter.MinPointMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0,
                MaxLOD = float.MaxValue
            };
            var sampler = device.Device.CreateSamplerState(samplerDes);
            
            context.PSSetSamplers(0, [sampler]);
            
            texture.BindTexture(context, 0);
        }

        base.Render(device);
        
        if (IndexBuffer == null)
            context.Draw((uint)_vertices.Length, 0);
        else
            context.DrawIndexed((uint)_indiecs.Length, 0, 0);
    }
}