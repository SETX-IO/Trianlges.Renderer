using System;
using Vortice.Direct3D;
using Vortice.WIC;
using Vortice.DXGI;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class TextureDx11
{
    private readonly ID3D11ShaderResourceView _textureView;
    
    public unsafe TextureDx11(ID3D11Device device, string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException($"{nameof(fileName)} is null or empty.");
        
        var decoder = UtiltDx.WicFactory.CreateDecoderFromFileName(fileName);
        var frameDecode = decoder.GetFrame(0);
        
        IWICFormatConverter converter = UtiltDx.CreateConverterFrame(frameDecode, frameDecode.PixelFormat, out Format format, out bool isConvert);
        
        uint rowBytes = UtiltDx.GetFrameDecodeRowBytes(frameDecode, format, out RectI rect, out byte[] textureCode);
        
        if (isConvert)
            converter.CopyPixels(rect, rowBytes, textureCode);
        else
            frameDecode.CopyPixels(rect, rowBytes, textureCode);
        
        SubresourceData data;
        fixed (void* ptr = textureCode)
            data = new SubresourceData(ptr, rowBytes);
        
        var desc = new Texture2DDescription(format, (uint)rect.Width, (uint)rect.Height, 1, 1);
        var texture = device.CreateTexture2D(desc, data);
        
        var srvDesc = new ShaderResourceViewDescription(ShaderResourceViewDimension.Texture2D, format);
        _textureView = device.CreateShaderResourceView(texture, srvDesc);
    }

    public void Bind(ID3D11DeviceContext context, uint slot = 0) =>
        context.PSSetShaderResource(slot, _textureView);
}