using System;
using Trianlges.Renderer.Backend.Direct3D11;
using Vortice.Direct3D11;

namespace Trianlges.Graphics.Direct3D11;

public class Material : IConfigMaterial, IBuildResource
{
    private Device3D _device;
    
    private ShaderProgame _progame = null!;
    private ID3D11RasterizerState _rasterizer = null!;
    private readonly ID3D11SamplerState _sampler;
    private TextureDx11? _texture;

    public bool FaceCull
    {
        set
        {
            var desc = _rasterizer.Description;
            _rasterizer = _device.Device.CreateRasterizerState(desc with { CullMode = value ? CullMode.Back : CullMode.None });
        }
    }

    public static IConfigMaterial Create(Device3D device3D)
    {
        var instance = new Material(device3D);
        return instance;
    }
    
    private Material(Device3D device3d)
    {
        var device = device3d;
        _device = device;
        
        var samDesc = new SamplerDescription(Filter.MinPointMagMipLinear, TextureAddressMode.Wrap, 0, 1, ComparisonFunction.Never, 0);
        _sampler = device.Device.CreateSamplerState(samDesc);
    }

    public IConfigMaterial SetShader(string path, InputElementDescription[] attirbutes)
    {
        _progame = ShaderProgame.Create(_device.Device)
            .Complier(path)
            .ConfigInput(attirbutes)
            .Build<ShaderProgame>();

        return this;
    }

    public IConfigMaterial SetTexture(string path)
    {
        _texture = _device.NewTexture(path);

        return this;
    }
    
    public IBuildResource ConfigRasterizer(bool isFaceCull, bool isFill)
    {
        var rasterizerDesc = new RasterizerDescription(isFaceCull ? CullMode.Back : CullMode.None, isFill ? FillMode.Solid : FillMode.Wireframe);
        _rasterizer = _device.Device.CreateRasterizerState(rasterizerDesc);

        return this;
    }

    public void Bind(ID3D11DeviceContext context, uint slot)
    {
        _progame.Bind(context);
        context.RSSetState(_rasterizer);
        context.PSSetSamplers(slot, [_sampler]);
        _texture?.Bind(context, slot);
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