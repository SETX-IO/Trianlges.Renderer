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
        IWICBitmapFrameDecode image = UtiltDx.LoadFrameForFile(fileName);
        IWICFormatConverter converter = UtiltDx.CreateConverterFrame(image, image.PixelFormat, out Format format, out bool isConvert);
        uint rowBytes = UtiltDx.GetFrameDecodeRowBytes(image, format, out RectI rect, out byte[] textureCode);
        
        if (isConvert)
            converter.CopyPixels(rect, rowBytes, textureCode);
        else
            image.CopyPixels(rect, rowBytes, textureCode);
        
        SubresourceData data;
        fixed (void* ptr = textureCode)
            data = new SubresourceData(ptr, rowBytes);
        
        Texture2DDescription desc = new(format, (uint)rect.Width, (uint)rect.Height, 1, 1);
        ID3D11Texture2D texture = device.CreateTexture2D(desc, data);
        
        ShaderResourceViewDescription srvDesc = new(ShaderResourceViewDimension.Texture2D, format);
        _textureView = device.CreateShaderResourceView(texture, srvDesc);
    }

    public void Bind(ID3D11DeviceContext context, uint slot = 0) =>
        context.PSSetShaderResource(slot, _textureView);
}