using System.Numerics;
using System.Runtime.CompilerServices;
using Vortice;
using Vortice.Direct3D11;
using Vortice.D3DCompiler;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics.Direct3D11;

public struct Vertex(Vector3 position, Vector3 color)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
}

public class Module : DrawElement
{
    public static readonly Module Trianlge;

    private Vertex[] _vertices = null!;
    private uint[]? _indiecs;
    
    static Module()
    {
        Trianlge = new Module();

        Vertex[] vertices =
        [
            new Vertex(new Vector3(0, 0.5f, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, 0), Vector3.UnitY),
            new Vertex(new Vector3(-0.5f, -0.5f, 0), Vector3.UnitZ)
        ];
        
        Trianlge.Init(vertices);
    }

    public void Init(Vertex[] vertices, uint[]? indiecs = null)
    {
        _vertices = vertices;
        _indiecs = indiecs;
    }

    public void CreateRenderResouces(ID3D11Device device)
    {
        var vBufferDesc =
            new BufferDescription((uint)(_vertices.Length * Unsafe.SizeOf<Vertex>()), BindFlags.VertexBuffer);

        var vData = DataStream.Create(_vertices, true, true);
        _vBuffer = device.CreateBuffer(vBufferDesc, vData);

         var vShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "vert", "vs_5_0");
         var pShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "frag", "ps_5_0");

         InputElementDescription[] inputDesc =
         [
             new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
             new InputElementDescription("COLOR", 0, Format.R32G32B32_Float, 12, 0),
         ];
         
         _vShader = device.CreateVertexShader(vShaderCode.Span);
         _pShader = device.CreatePixelShader(pShaderCode.Span);

         _vShaderLayout = device.CreateInputLayout(inputDesc, vShaderCode.Span);
    }

    public override void Render(D3DDevice device)
    {
        var context = device.DContext;
        
        if (context == null) return;
        if (_vBuffer == null && _vShader == null && _pShader == null)
            CreateRenderResouces(device.Device!);
        
        base.Render(device);
        
        context.Draw((uint)_vertices.Length, 0);
    }
}