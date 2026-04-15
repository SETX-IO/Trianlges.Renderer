using System.Numerics;
using SharpGen.Runtime;
using Triangles.Core.Renderer;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Triangles.Core.RenderBg;

public class CommandBuffer(ID3D12GraphicsCommandList commandList) : ICommandBuffer
{
    public readonly ID3D12GraphicsCommandList4 CommandList = commandList.As<ID3D12GraphicsCommandList4>();
    private RenderTargetTexture _renderTarget = null!;
    private bool _isPresent;
    private bool _useIndexBuffer;

    private uint _vBufferCount;
    private uint _iBufferCount;

    public CommandBuffer CmdBuffer => this;

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

    public void SetTarget(RenderTargetTexture renderTargetView, CpuDescriptorHandle? depthStencilDescriptor = null)
    {
        _renderTarget = renderTargetView;
    }
        
    public void ClearTarget(Color4 clearColor, PrimitiveTopology topology)
    {
        RenderPassRenderTargetDescription rtInfo = new RenderPassRenderTargetDescription(_renderTarget.ResourceHandel,
            new RenderPassBeginningAccess(new ClearValue(Format.R8G8B8A8_SNorm, clearColor)),
            new RenderPassEndingAccess(RenderPassEndingAccessType.Preserve));
        
        _renderTarget.Lock(this);
        
        CommandList.BeginRenderPass(rtInfo);
        
        CommandList.IASetPrimitiveTopology(topology);
    }

    public void ResourceBarrier(ID3D12Resource resource)
    {
        ResourceStates stateBefore = _isPresent ? ResourceStates.RenderTarget : ResourceStates.Present;
        ResourceStates stateAfter = _isPresent ? ResourceStates.Present : ResourceStates.RenderTarget;
        CommandList.ResourceBarrierTransition(resource, stateBefore, stateAfter);

        _isPresent = !_isPresent;
    }
    
    public void ResourceBarrier(ID3D12Resource resource, ResourceStates stateBefore, ResourceStates stateAfter) =>
        CommandList.ResourceBarrierTransition(resource, stateBefore, stateAfter);
    
    public void ApplyPipeLine(IPipeLine pipeLine)
    {
        pipeLine.ApplyPipeLine(this);
    }

    public void SetBuffer(VertexBufferView[] vBufferViews, uint vBufferSlot = 0, IndexBufferView? indexBufferView = null)
    {
        _vBufferCount = vBufferViews[0].SizeInBytes / vBufferViews[0].StrideInBytes;
        CommandList.IASetVertexBuffers(vBufferSlot, vBufferViews);
        
        if (indexBufferView.HasValue)
        {
            CommandList.IASetIndexBuffer(indexBufferView);

            _iBufferCount = (uint)(indexBufferView.Value.SizeInBytes / (indexBufferView.Value.Format == Format.R32_UInt ? 4 : 2));
        }
        
        _useIndexBuffer = indexBufferView.HasValue;
    }

    public void SetConstantBuffer(ID3D12DescriptorHeap descHeap)
    {
        CommandList.SetDescriptorHeaps(descHeap);

        var heapGpuHandel = descHeap.GetGPUDescriptorHandleForHeapStart();

        CommandList.SetGraphicsRootDescriptorTable(0, heapGpuHandel);

        heapGpuHandel = heapGpuHandel.Offset(1, descHeap.GetDevice<ID3D12Device>().GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView));
        
        CommandList.SetGraphicsRootDescriptorTable(1, heapGpuHandel);
    }

    public void Draw(uint instanceCount = 1)
    {
        if (_useIndexBuffer)
            CommandList.DrawIndexedInstanced(_iBufferCount, instanceCount, 0, 0, 0);
        else
            CommandList.DrawInstanced(_vBufferCount, instanceCount, 0, 0);
        
        CommandList.EndRenderPass();
        
        _renderTarget.Unlock(this);
    }

    public void CloseCmd()
    {
        CommandList.Close();
    }

    public void Dispose()
    {
        CommandList.Dispose();
    }
}