using Vortice.Direct3D11;

namespace Trianlges.Renderer.Type;

public struct BlendOptions
{ 
    public bool BlendEnable = false;
    public ColorWriteEnable WriteMask = ColorWriteEnable.All;
    
    public BlendOperation RgbBlendOperation = BlendOperation.Add;
    public BlendOperation AlphaBlendOperation = BlendOperation.Add;

    public Blend SourceRgbBlendFactor = Blend.One;
    public Blend DestinationRgbBlendFactor = Blend.Zero;
    
    public Blend SourceAlphaBlendFactor = Blend.One;
    public Blend DestinationAlphaBlendFactor = Blend.Zero;

    public BlendOptions()
    {
    }
}

public struct PipeLineDescriptor
{
    public BlendOptions BlendOptions;
}