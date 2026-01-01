using System;
using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Material : IConfigMaterial, IBuildResource
{
    private ID3D11Device _device;
    
    private ShaderProgame _progame = null!;
    private ID3D11RasterizerState _rasterizer = null!;
    private readonly ID3D11SamplerState _sampler;
    private Texture? _texture;

    public bool FaceCull
    {
        set
        {
            var desc = _rasterizer.Description;
            _rasterizer = _device.CreateRasterizerState(desc with { CullMode = value ? CullMode.Back : CullMode.None });
        }
    }

    public static IConfigMaterial Create(IDevice3D device3D)
    {
        var instance = new Material(device3D);
        return instance;
    }
    
    private Material(IDevice3D device3d)
    {
        var device = device3d.Device;
        _device = device;
        
        var samDesc = new SamplerDescription(Filter.MinPointMagMipLinear, TextureAddressMode.Wrap, 0, 1, ComparisonFunction.Never, 0);
        _sampler = device.CreateSamplerState(samDesc);
    }

    public IConfigMaterial SetShader(string path, InputElementDescription[] attirbutes)
    {
        _progame = ShaderProgame.Create(_device)
            .Complier(path)
            .ConfigInput(attirbutes)
            .Build<ShaderProgame>();

        return this;
    }

    public IConfigMaterial SetTexture(string path)
    {
        _texture = Texture.Create(_device)
            .LoadFormFile(path)
            .Build<Texture>();

        return this;
    }

    
    public IBuildResource ConfigRasterizer(bool isFaceCull, bool isFill)
    {
        var rasterizerDesc = new RasterizerDescription(isFaceCull ? CullMode.Back : CullMode.None, isFill ? FillMode.Solid : FillMode.Wireframe);
        _rasterizer = _device.CreateRasterizerState(rasterizerDesc);

        return this;
    }

    public void Bind(ID3D11DeviceContext context, uint slot)
    {
        _progame.Bind(context);
        context.RSSetState(_rasterizer);
        context.PSSetSamplers(slot, [_sampler]);
        _texture?.BindTexture(context, slot);
    }
}

public interface IConfigMaterial
{
    IConfigMaterial SetShader(string path, InputElementDescription[] attirbutes)
    {
        throw new NotImplementedException();
    }

    IConfigMaterial SetTexture(string path)
    {
        throw new NotImplementedException();
    }

    IBuildResource ConfigRasterizer(bool isFaceCull, bool isFill)
    {
        throw new NotImplementedException();
    }
}