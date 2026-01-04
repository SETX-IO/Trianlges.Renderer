using System;
using System.IO;
using Vortice.Direct3D11;
using Vortice.D3DCompiler;

namespace Trianlges.Graphics.Direct3D11;

public class ShaderProgame : ICompilerShader, IConfigShader, IBuildResource
{
    private readonly ID3D11Device _refDevice;
    private ReadOnlyMemory<byte> _vertextShaderCode;
    public ID3D11PixelShader? PixelShader { get; protected set; }
    public ID3D11VertexShader? VertexShader { get; protected set; }
    public ID3D11InputLayout? VertexLayout { get; protected set; }

    private ShaderProgame(ID3D11Device device)
    {
        _refDevice = device;
    }
    
    public static ICompilerShader Create(ID3D11Device device)
    {
        var instance = new ShaderProgame(device);
        return instance;
    }
    
    private ReadOnlyMemory<byte> CompilerShader(string shaderPath, string entryPoint, string profile)
    {
        ReadOnlyMemory<byte> shaderCode;

#if DEBUG
        shaderCode = Compiler.CompileFromFile(shaderPath, entryPoint, profile);
#else

        string path;
        var cacheFile = $"{entryPoint}_{Path.GetFileNameWithoutExtension(shaderPath)}.psv";
        
        try
        {
            path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(shaderPath)), cacheFile);
        }
        catch (Exception e) when(e is ArgumentException or ArgumentException)
        {
            Console.WriteLine(e);
            path = cacheFile;
        }

        if (!File.Exists(path))
        {
            shaderCode = Compiler.CompileFromFile(shaderPath, entryPoint, profile);
            using var stream = File.Create(path);

            stream.Write(shaderCode.Span);
        }
        else shaderCode = File.ReadAllBytes(path);
#endif
        return shaderCode;
    }

    public IConfigShader Complier(string shaderPath)
    {
        var vCode = CompilerShader(shaderPath, "vert", "vs_5_0");
        var pCode = CompilerShader(shaderPath, "frag", "ps_5_0");

        _vertextShaderCode = vCode;
        
        VertexShader = _refDevice.CreateVertexShader(vCode.Span);
        PixelShader = _refDevice.CreatePixelShader(pCode.Span);

        return this;
    }

    public IConfigShader Complier(string vShader, string pShader)
    {
        var vCode = CompilerShader(vShader, "vert", "vs_5_0");
        var pCode = CompilerShader(pShader, "frag", "ps_5_0");

        _vertextShaderCode = vCode;
        
        VertexShader = _refDevice.CreateVertexShader(vCode.Span);
        PixelShader = _refDevice.CreatePixelShader(pCode.Span);

        return this;
    }

    public IBuildResource ConfigInput(InputElementDescription[] vertextInput)
    {
        VertexLayout = _refDevice.CreateInputLayout(vertextInput, _vertextShaderCode.Span);
        
        return this;
    }

    public void Bind(ID3D11DeviceContext context)
    {
        context.VSSetShader(VertexShader);
        context.PSSetShader(PixelShader);
        
        context.IASetInputLayout(VertexLayout);
    }
}

public interface ICompilerShader
{
    IConfigShader Complier(string shaderPath)
    {
        throw new NotImplementedException();
    }
    
    IConfigShader Complier(string vShader, string pShader)
    {
        throw new NotImplementedException();
    }
}

public interface IConfigShader
{
    IBuildResource ConfigInput(InputElementDescription[] vertextInput)
    {
        throw new NotImplementedException();
    }
}