using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public class Fence
{
    private readonly ID3D12CommandQueue _queue;
    private readonly AutoResetEvent _event;
    private readonly ID3D12Fence _fence;

    private uint _frameCount;
    private uint _frameBufferCount;
    
    public Fence(Device device, uint bufferCount)
    {
        _frameBufferCount = bufferCount;
        _queue = device.CommandQueue;
        _fence = device.CreateFence();
        
        _event = new AutoResetEvent(false);
    }

    public void Wait(bool isRender = false)
    {
        _queue.Signal(_fence, ++_frameCount);
        
        ulong fenceValue = _fence.CompletedValue;
        
        if (fenceValue - _frameCount >= _frameBufferCount && isRender)
        {
            _fence.SetEventOnCompletion(fenceValue + 1, _event);
            _event.WaitOne();
            
            return;
        }
        
        _fence.SetEventOnCompletion(_frameCount, _event);
        _event.WaitOne();
    }
}