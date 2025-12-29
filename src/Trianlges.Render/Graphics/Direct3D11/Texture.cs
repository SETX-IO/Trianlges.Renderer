using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Vortice.WIC;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Texture : ILoadTexture, IBuildResource
{
    private static IWICImagingFactory WicFactory { get; } = new();
    
    private readonly ID3D11Device _refDevice;
    
    private ID3D11ShaderResourceView _textureView = null!;

    private Texture(ID3D11Device device)
    {
        _refDevice = device;
    }
    
    public static ILoadTexture Create(ID3D11Device device)
    {
        var instance = new Texture(device);
        return instance;
    }

    public IBuildResource LoadFormFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException($"{fileName} is null or empty.");
        
        var decoder = WicFactory.CreateDecoderFromFileName(fileName);
        var frameDecode = decoder.GetFrame(0);
        var bbp = frameDecode.PixelFormat == PixelFormat.Format128bppRGBAFloat ? 128 : 96;

        var frameSize = frameDecode.Size;
        var rowbytes = (frameSize.Width * bbp + 7) / 8;
        var numBytes = rowbytes * frameSize.Height;
        
        var textureCode = new byte[numBytes];
        frameDecode.CopyPixels(new RectI(frameSize.Width, frameSize.Height), (uint)rowbytes, textureCode);
        
        var desc = new Texture2DDescription(Format.B8G8R8A8_UNorm, (uint)frameSize.Width, (uint)frameSize.Height);
        var texture = _refDevice.CreateTexture2D(desc);
        _refDevice.ImmediateContext.UpdateSubresource(textureCode, texture);

        var srvDesc = new ShaderResourceViewDescription(ShaderResourceViewDimension.Texture2D, desc.Format);
        _textureView = _refDevice.CreateShaderResourceView(texture, srvDesc);
        
        return this;
    }

    public T Build<T>() where T : class, IBuildResource
    {
        var instnce = this as T;
        return instnce ?? throw new TypeAccessException();
    }

    public void BindTexture(ID3D11DeviceContext context, uint slot)
    {
        context.PSSetShaderResource(slot, _textureView);
    }
}

public interface ILoadTexture
{
    IBuildResource LoadFormFile(string fileName)
    {
        throw new NotImplementedException();
    }

    IBuildResource LoadFormStream()
    {
        throw new NotImplementedException();
    }
}