using System;
using Vortice.Direct3D11;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class ShaderDx11
{
    private readonly ID3D11DeviceChild _shader;
    public readonly ShaderType ShaderType;
    public ReadOnlyMemory<byte> ShaderModule;
    
    public ShaderDx11(ID3D11Device device, ShaderType type, string fileName, string entryPoint = "main", ShaderVersion version = ShaderVersion.V5)
    {
        ShaderType = type;
        ShaderModule = UtiltDx.CompilerShader(fileName, entryPoint, type, version);
        _shader = CreateShader(device);
    }

    public void Bind(ID3D11DeviceContext context)
    {
        switch (_shader)
        {
            case ID3D11VertexShader shader : context.VSSetShader(shader); break;
            case ID3D11HullShader shader: context.HSSetShader(shader); break;
            case ID3D11DomainShader shader : context.DSSetShader(shader); break;
            case ID3D11GeometryShader shader: context.GSSetShader(shader); break;
            case ID3D11PixelShader shader : context.PSSetShader(shader); break;
            case ID3D11ComputeShader shader: context.CSSetShader(shader); break;
        }
    }
    
    private ID3D11DeviceChild CreateShader(ID3D11Device device) 
    {
        ID3D11DeviceChild shader = ShaderType switch
        {
            ShaderType.Vertex => device.CreateVertexShader(ShaderModule.Span),
            ShaderType.Hull => device.CreateHullShader(ShaderModule.Span),
            ShaderType.Domain => device.CreateDomainShader(ShaderModule.Span),
            ShaderType.Geometry => device.CreateGeometryShader(ShaderModule.Span),
            ShaderType.Pixel => device.CreatePixelShader(ShaderModule.Span),
            ShaderType.Compute => device.CreateComputeShader(ShaderModule.Span),
            _ => throw new ArgumentOutOfRangeException(nameof(ShaderType), ShaderType, null)
        };

        return shader;
    }
}