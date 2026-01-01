using System;
using System.Linq;
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
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException($"{nameof(fileName)} is null or empty.");
        
        var decoder = WicFactory.CreateDecoderFromFileName(fileName);
        var frameDecode = decoder.GetFrame(0);
 
        var format = PixleFormatToDxgiFormat(frameDecode.PixelFormat);

        bool isConvert = false;
        var converter = WicFactory.CreateFormatConverter();
        if (format == Format.Unknown)
        {
            var convertFormat = GetConvertFormat(frameDecode.PixelFormat);
            if (convertFormat == PixelFormat.FormatDontCare)
                throw new NotSupportedException($"\"{fileName}\" image format is not support.");
            
            format = PixleFormatToDxgiFormat(convertFormat);
            bool canConvert = converter.CanConvert(frameDecode.PixelFormat, convertFormat);
            if (canConvert)
            {
                converter.Initialize(frameDecode, convertFormat);
            }

            isConvert = canConvert;
        }
        
        var frameSize = frameDecode.Size;
        var bbp = DxgiFormatTobbp(format);
        var rowbytes = frameSize.Width * bbp / 8;
        var numBytes = rowbytes * frameSize.Height;
        
        var textureCode = new byte[numBytes];
        frameDecode.CopyPixels(new RectI(frameSize.Width, frameSize.Height), (uint)rowbytes, textureCode);

        if (isConvert)
        {
            converter.CopyPixels(new RectI(frameSize.Width, frameSize.Height), (uint)rowbytes, textureCode);
        }
        else
        {
            frameDecode.CopyPixels(new RectI(frameSize.Width, frameSize.Height), (uint)rowbytes, textureCode);
        }


        var desc = new Texture2DDescription(format, (uint)frameSize.Width, (uint)frameSize.Height, 1, 1);
        var texture = _refDevice.CreateTexture2D(desc);
        _refDevice.ImmediateContext.UpdateSubresource(textureCode, texture);
        
        var srvDesc = new ShaderResourceViewDescription(ShaderResourceViewDimension.Texture2D, format);
        _textureView = _refDevice.CreateShaderResourceView(texture, srvDesc);
        
        return this;
    }

    public void BindTexture(ID3D11DeviceContext context, uint slot)
    {
        context.PSSetShaderResource(slot, _textureView);
    }
    
    private Format PixleFormatToDxgiFormat(Guid pixelFormat)
    {
        (Guid pixelFormat, Format dxgiFormat)[] toArray =
        [
            (PixelFormat.Format128bppRGBAFloat, Format.R32G32B32A32_Float),
            (PixelFormat.Format64bppRGBAHalf, Format.R16G16B16A16_Float),
            (PixelFormat.Format64bppRGBA, Format.R16G16B16A16_UNorm),
            (PixelFormat.Format32bppRGBA, Format.R8G8B8A8_UNorm),
            (PixelFormat.Format32bppBGRA, Format.B8G8R8A8_UNorm),
            (PixelFormat.Format32bppBGR, Format.B8G8R8X8_UNorm),
            (PixelFormat.Format32bppRGBA1010102XR, Format.R10G10B10_Xr_Bias_A2_UNorm),
            (PixelFormat.Format32bppRGBA1010102, Format.R10G10B10A2_UNorm),
            (PixelFormat.Format32bppRGBE, Format.R9G9B9E5_SharedExp),
            (PixelFormat.Format16bppBGRA5551, Format.B5G5R5A1_UNorm),
            (PixelFormat.Format16bppBGR565, Format.B5G6R5_UNorm),
            (PixelFormat.Format32bppGrayFloat, Format.R32_Float),
            (PixelFormat.Format16bppGrayHalf, Format.R16_Float),
            (PixelFormat.Format16bppGray, Format.R16_UNorm),
            (PixelFormat.Format8bppGray, Format.R8_UNorm),
            (PixelFormat.Format8bppAlpha, Format.A8_UNorm)
        ];

        return (from fmt in toArray where pixelFormat == fmt.pixelFormat select fmt.dxgiFormat).FirstOrDefault();
    }

    private Guid GetConvertFormat(Guid sourceFormat)
    {
        (Guid source, Guid target)[] converts =
        [
            (PixelFormat.FormatBlackWhite, PixelFormat.Format8bppGray),
            
            (PixelFormat.Format1bppIndexed, PixelFormat.Format32bppBGRA),
            (PixelFormat.Format2bppIndexed, PixelFormat.Format32bppBGRA),
            (PixelFormat.Format4bppIndexed, PixelFormat.Format32bppBGRA),
            (PixelFormat.Format8bppIndexed, PixelFormat.Format32bppBGRA),
            
            (PixelFormat.Format2bppGray, PixelFormat.Format8bppGray),
            (PixelFormat.Format4bppGray, PixelFormat.Format8bppGray),
            
            (PixelFormat.Format16bppGrayFixedPoint, PixelFormat.Format16bppGrayHalf),
            (PixelFormat.Format32bppGrayFixedPoint, PixelFormat.Format32bppGrayFloat),
            
            (PixelFormat.Format16bppBGR555, PixelFormat.Format16bppBGRA5551),
            
            (PixelFormat.Format32bppBGR101010, PixelFormat.Format32bppRGBA1010102),
            
            (PixelFormat.Format24bppBGR, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format24bppRGB, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format32bppPBGRA, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format32bppPRGBA, PixelFormat.Format32bppRGBA),
            
            (PixelFormat.Format48bppRGB, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format48bppBGR, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format64bppBGRA, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format64bppPRGBA, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format64bppPBGRA, PixelFormat.Format64bppRGBA),
            
            (PixelFormat.Format48bppRGBFixedPoint, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format48bppBGRFixedPoint, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format64bppRGBAFixedPoint, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format64bppBGRAFixedPoint, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format64bppRGBFixedPoint, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format64bppRGBHalf, PixelFormat.Format64bppRGBAHalf),
            (PixelFormat.Format48bppRGBHalf, PixelFormat.Format64bppRGBAHalf),
            
            (PixelFormat.Format128bppPRGBAFloat, PixelFormat.Format128bppRGBAFloat),
            (PixelFormat.Format128bppRGBFloat, PixelFormat.Format128bppRGBAFloat),
            (PixelFormat.Format128bppRGBAFixedPoint, PixelFormat.Format128bppRGBAFloat),
            (PixelFormat.Format128bppRGBFixedPoint, PixelFormat.Format128bppRGBAFloat),
            (PixelFormat.Format32bppRGBE, PixelFormat.Format128bppRGBAFloat),
            
            (PixelFormat.Format32bppCMYK, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format64bppCMYK, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format40bppCMYKAlpha, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format80bppCMYKAlpha, PixelFormat.Format64bppRGBA),
            
            (PixelFormat.Format32bppRGB, PixelFormat.Format32bppRGBA),
            (PixelFormat.Format64bppRGB, PixelFormat.Format64bppRGBA),
            (PixelFormat.Format64bppPRGBAHalf, PixelFormat.Format64bppRGBAHalf),
        ];

        foreach (var convert in converts)
        {
            if (convert.source == sourceFormat)
            {
                return convert.target;
            }
        }
        
        return  PixelFormat.FormatDontCare;
    }

    private int DxgiFormatTobbp(Format dxgiFormat) => dxgiFormat switch
    {
        Format.R32G32B32A32_Float => 128,
        Format.R16G16B16A16_Float or Format.R16G16B16A16_UNorm => 64,
        Format.R8G8B8A8_UNorm or Format.B8G8R8A8_UNorm or Format.B8G8R8X8_UNorm or 
            Format.R10G10B10_Xr_Bias_A2_UNorm or Format.R10G10B10A2_UNorm or Format.R32_Float => 32,
        Format.B5G5R5A1_UNorm or Format.B5G6R5_UNorm or Format.R16_Float or Format.R16_SNorm => 16,
        Format.R8_UNorm or Format.A8_UNorm => 8,
        _ => throw new ArgumentOutOfRangeException(nameof(dxgiFormat), dxgiFormat, null)
    };
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