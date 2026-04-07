using System.IO.Pipelines;
using System.Numerics;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.Mathematics;

namespace Triangles.Core.RenderBg;

public class CommandBuffer(ID3D12GraphicsCommandList commandList)
{
    public readonly ID3D12GraphicsCommandList CommandList = commandList;
    private CpuDescriptorHandle _renderTargetViewHandle;
    private bool _isPresent;
    private bool _useIndexBuffer;
    
    public void SetViewProt(USize size, Vector2 position = default)
    {
        Viewport viewport = new Viewport
        {
            Width = size.Width,
            Height = size.Height,
            X = position.X,
            Y = position.Y
        };

        CommandList.RSSetViewports(viewport);
    }

    public void SetScissor(USize size)
    {
        RawRect rect = new RawRect(0, 0, (int)size.Width, (int)size.Height);
        CommandList.RSSetScissorRects(rect);
    }

    public void SetTarget(CpuDescriptorHandle renderTargetView, CpuDescriptorHandle? depthStencilDescriptor = null)
    {
        _renderTargetViewHandle = renderTargetView;
        
        CommandList.OMSetRenderTargets(renderTargetView, depthStencilDescriptor);
    }
        
    public void ClearTarget(Color4 clearColor, PrimitiveTopology topology)
    {
        CommandList.ClearRenderTargetView(_renderTargetViewHandle, clearColor);
        
        CommandList.IASetPrimitiveTopology(topology);
    }

    public void ResourceBarrier(ID3D12Resource resource)
    {
        ResourceStates stateBefore = _isPresent ? ResourceStates.RenderTarget : ResourceStates.Present;
        ResourceStates stateAfter = _isPresent ? ResourceStates.Present : ResourceStates.RenderTarget;
        CommandList.ResourceBarrierTransition(resource, stateBefore, stateAfter);

        _isPresent = !_isPresent;
    }
    
    public void ApplyPipeLine(PipeLine pipeLine)
    {
        pipeLine.ApplyPipeLine(this);
    }

    public void SetBuffer(VertexBufferView[] vBufferViews, uint vBufferSlot = 0, IndexBufferView? indexBufferView = null)
    {
        CommandList.IASetVertexBuffers(vBufferSlot, vBufferViews);

        if (indexBufferView.HasValue)
            CommandList.IASetIndexBuffer(indexBufferView);
        
        _useIndexBuffer = indexBufferView.HasValue;
    }

    public void Draw(uint instanceCount = 1)
    {
        if (_useIndexBuffer)
            CommandList.DrawIndexedInstanced(6, instanceCount, 0, 0, 0);
        else
            CommandList.DrawInstanced(6, instanceCount, 0, 0);

    }

    public void Release()
    {
        // _renderTargetViewHandle.Ptr = 0;
    }
}