using Vortice.Mathematics;

namespace Trianlges.Renderer.Backend;

public struct RenderPassDescriptor
{
    public float ClearDepthValue = 0f;
    public float ClearStencilValue = 0f;
    public Color4 ClearColor = new(0);
    public bool NeedColorAttachment = true;
    public bool DepthTestEnable = false;
    public bool StencilTestEnable = false;
    public bool NeedClearColor = false;
    public bool NeedClearDepth = false;
    public bool NeedClearStencil = false;

    public RenderPassDescriptor()
    {
    }

    public bool NeedDepthStencilAttachment() => DepthTestEnable || StencilTestEnable;

    public bool Equals(RenderPassDescriptor other)
    {
        return ClearDepthValue.Equals(other.ClearDepthValue) &&
               ClearStencilValue.Equals(other.ClearStencilValue) &&
               ClearColor.Equals(other.ClearColor) &&
               NeedColorAttachment == other.NeedColorAttachment &&
               DepthTestEnable == other.DepthTestEnable &&
               StencilTestEnable == other.StencilTestEnable &&
               NeedClearColor == other.NeedClearColor &&
               NeedClearDepth == other.NeedClearDepth &&
               NeedClearStencil == other.NeedClearStencil;
    }

    public static bool operator==(RenderPassDescriptor self, RenderPassDescriptor other)
    {
        return self.Equals(other);
    }

    public static bool operator !=(RenderPassDescriptor self, RenderPassDescriptor other)
    {
        return !self.Equals(other);
    }
}