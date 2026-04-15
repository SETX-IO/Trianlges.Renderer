using Triangles.Core.RenderBg;

namespace Triangles.Core.Renderer;

public interface IPipeLine : IContextBase
{
    void ApplyPipeLine(CommandBuffer cmdBuffer);
}