using Vortice.Direct3D11;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class ProgramDx11 : IProgram
{
    public ShaderDx11 VertexShader;
    public ShaderDx11 PixelShader;
    public ID3D11InputLayout Layout;
    public readonly InputElementDescription[] AttributeInfos;
    
    public ProgramDx11(ID3D11Device device, string vShaderName, string pShaderName, InputElementDescription[]? attributeInfos = null)
    {
        VertexShader = new ShaderDx11(device, ShaderType.Vertex, vShaderName, "vert");
        PixelShader = new ShaderDx11(device, ShaderType.Pixel, pShaderName, "frag");

        AttributeInfos = attributeInfos ?? Vertex.VertexInputLayout;
        
        Layout = device.CreateInputLayout(AttributeInfos, VertexShader.ShaderModule.Span);
    }

    public void Bind(ID3D11DeviceContext context)
    {
        VertexShader.Bind(context);
        PixelShader.Bind(context);
        
        context.IASetInputLayout(Layout);
    }
}