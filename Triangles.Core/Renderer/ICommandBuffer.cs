using System.Numerics;
using Triangles.Core.RenderBg;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.Mathematics;

namespace Triangles.Core.Renderer;

public interface ICommandBuffer : IContextBase
{
    public CommandBuffer CmdBuffer { get; }
    public void SetViewProt(USize size, Vector2 position = default);
    public void SetScissor(USize size);
    public void SetTarget(RenderTargetTexture renderTargetView, CpuDescriptorHandle? depthStencilDescriptor = null);
    public void ClearTarget(Color4 clearColor, PrimitiveTopology topology);
    public void ResourceBarrier(ID3D12Resource resource);
    public void ResourceBarrier(ID3D12Resource resource, ResourceStates stateBefore, ResourceStates stateAfter);
    public void ApplyPipeLine(IPipeLine pipeLine);
    public void SetBuffer(VertexBufferView[] vBufferViews, uint vBufferSlot = 0,
        IndexBufferView? indexBufferView = null);

    public void SetConstantBuffer(ID3D12DescriptorHeap descHeap);
    public void Draw(uint instanceCount = 1);
    public void CloseCmd();
}