using System;
using System.Linq;
using Vortice.DXGI;
using Vortice.Mathematics;
using Vortice.WIC;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class UtiltDx
{
    public static IWICImagingFactory WicFactory { get; } = new();

    public static IWICFormatConverter CreateConverterFrame(IWICBitmapFrameDecode frameDecode, Guid srcFormat, out Format dxgiFormat, out bool isCanConvert)
    {
        IWICFormatConverter converter = WicFactory.CreateFormatConverter();
        dxgiFormat = PixleFormatToDxgiFormat(frameDecode.PixelFormat);
        isCanConvert = false;

        if (dxgiFormat != Format.Unknown) return converter;
        
        Guid format = GetConvertFormat(srcFormat);
        
        if (format == PixelFormat.FormatDontCare)
            throw new NotSupportedException($"\"{frameDecode}\" image format is not support.");

        dxgiFormat = PixleFormatToDxgiFormat(format);
        
        isCanConvert = converter.CanConvert(frameDecode.PixelFormat, format);
        if (isCanConvert)
            converter.Initialize(frameDecode, format);

        return converter;
    }

    public static uint GetFrameDecodeRowBytes(IWICBitmapFrameDecode frameDecode, Format dxgiFormat, out RectI rect, out byte[] pixels)
    {
        SizeI frameSize = frameDecode.Size;
        int bbp = DxgiFormatTobbp(dxgiFormat);
        int rowBytes = frameSize.Width * bbp / 8;
        int numBytes = rowBytes * frameSize.Height;

        pixels = new byte[numBytes];
        
        rect = new RectI(frameSize.Width, frameSize.Height);
        
        return (uint)rowBytes;
    }

    private static Format PixleFormatToDxgiFormat(Guid pixelFormat)
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

    private static Guid GetConvertFormat(Guid sourceFormat)
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

    private static int DxgiFormatTobbp(Format dxgiFormat) => dxgiFormat switch
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