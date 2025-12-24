using System.Numerics;
using Vortice;
using Vortice.D3DCompiler;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Module : DrawElement
{
    public static readonly Module Trianlge;
    public static readonly Module Quadrilateral;
    public static readonly Module Cube;
    
    private uint[] _indiecs = [];

    private Vertex[] _vertices = null!;

    static Module()
    {
        Trianlge = new Module();
        Quadrilateral = new Module();
        Cube = new Module();

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
            new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), Vector3.Zero),
            new Vertex(new Vector3(-1.0f, 1.0f, -1.0f), Vector3.UnitX),
            new Vertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 0.0f)),
            new Vertex(new Vector3(1.0f, -1.0f, -1.0f), Vector3.UnitY),
            new Vertex(new Vector3(-1.0f, -1.0f, 1.0f), Vector3.UnitZ),
            new Vertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f)),
            new Vertex(new Vector3(1.0f, 1.0f, 1.0f), Vector3.One),
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

    public void Init(Vertex[] vertices, uint[] indiecs)
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
        _vBuffer = device.CreateBuffer(vBufferDesc, vData);

        if (_indiecs is not { Length: 0 })
        {
            var iBufferDesc =
                new BufferDescription((uint)(_indiecs.Length * sizeof(uint)), BindFlags.IndexBuffer,
                    ResourceUsage.Immutable);

            var iData = DataStream.Create(_indiecs, true, true);
            _iBuffer = device.CreateBuffer(iBufferDesc, iData);
        }

        var vShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "vert", "vs_5_0");
        var pShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "frag", "ps_5_0");

        InputElementDescription[] inputDesc =
        [
            new("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            new("COLOR", 0, Format.R32G32B32_Float, 12, 0)
        ];

        _vShader = device.CreateVertexShader(vShaderCode.Span);
        _pShader = device.CreatePixelShader(pShaderCode.Span);

        _vShaderLayout = device.CreateInputLayout(inputDesc, vShaderCode.Span);
    }

    public override void Render(IDevice3D device)
    {
        var context = device.DContext;

        if (_vShader == null && _pShader == null)
            CreateRenderResouces(device.Device);

        base.Render(device);
        
        if (_iBuffer == null)
            context.Draw((uint)_vertices.Length, 0);
        else
            context.DrawIndexed((uint)_indiecs.Length, 0, 0);
    }
}