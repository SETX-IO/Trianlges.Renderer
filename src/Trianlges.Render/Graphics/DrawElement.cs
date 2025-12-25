using Trianlges.Render.Graphics.Direct3D11;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public abstract class DrawElement
{
    public ID3D11Buffer VertextBuffer { get; protected set; } = null!;
    public ID3D11Buffer? IndexBuffer { get; protected set; }
    public ID3D11PixelShader? PixelShader { get; protected set; }
    public ID3D11VertexShader? VertexShader { get; protected set; }
    public ID3D11InputLayout? VertextLayout { get; protected set; }

    public void Init(IDevice3D device3D)
    {
        var device = device3D.Device;
        
        var vShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "vert", "vs_5_0");
        var pShaderCode = Compiler.CompileFromFile("Assets/Shader.hlsl", "frag", "ps_5_0");

        InputElementDescription[] inputDesc =
        [
            VertexInputElement.Position,
            VertexInputElement.Color3
        ];

        VertexShader = device.CreateVertexShader(vShaderCode.Span);
        PixelShader = device.CreatePixelShader(pShaderCode.Span);

        VertextLayout = device.CreateInputLayout(inputDesc, vShaderCode.Span);
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

        context.VSSetShader(VertexShader);
        context.PSSetShader(PixelShader);
        
        context.IASetVertexBuffers(0, 1, [VertextBuffer], [stride], [offset]);
        
        context.IASetInputLayout(VertextLayout);
        if (IndexBuffer != null) context.IASetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
    }
}