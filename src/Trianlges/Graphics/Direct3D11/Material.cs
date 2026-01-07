using System;
using Trianlges.Renderer.Backend.Direct3D11;
using Vortice.Direct3D11;

namespace Trianlges.Graphics.Direct3D11;

public class Material : IConfigMaterial, IBuildResource
{
    private readonly Device3D _device;
    
    private ShaderPrograme _programe = null!;
    private TextureDx11? _texture;
    
    public static IConfigMaterial Create(Device3D device3D)
    {
        var instance = new Material(device3D);
        return instance;
    }
    
    private Material(Device3D device3d)
    {
        var device = device3d;
        _device = device;
    }

    public IBuildResource SetShader(string path, InputElementDescription[] attirbutes)
    {
        _programe = ShaderPrograme.Create(_device.Device)
            .Complier(path)
            .ConfigInput(attirbutes)
            .Build<ShaderPrograme>();

        return this;
    }

    public void SetTexture(string path)
    {
        _texture = _device.NewTexture(path);
    }

    public void Bind(ID3D11DeviceContext context, uint slot)
    {
        _device.RenderPipeLine.Bind(context);
        _programe.Bind(context);
        _texture?.Bind(context, slot);
    }
}

public interface IConfigMaterial
{
    IBuildResource SetShader(string path, InputElementDescription[] attirbutes)
    {
        throw new NotImplementedException();
    }
}