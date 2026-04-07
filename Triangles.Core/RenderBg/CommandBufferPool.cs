using System.Runtime.InteropServices;
using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public class CommandBufferPool
{
    private readonly ID3D12Device _device;
    private readonly ID3D12CommandAllocator[] _allocator;
    private readonly Dictionary<string, CommandBuffer> _cmdListsCache;
    private readonly uint _allocationCount;
        
    public CommandBufferPool(ID3D12Device device, uint allocationCount)
    {
        _device = device;
        _allocationCount = allocationCount;
        _allocator = new ID3D12CommandAllocator[allocationCount];
        _cmdListsCache = new Dictionary<string, CommandBuffer>(16);
        
        for (int i = 0; i < allocationCount; i++)
        {
            _allocator[i] = device.CreateCommandAllocator(CommandListType.Direct);
        }
        
        device.AddRef();
    }

    private uint frameCount;
    
    public CommandBuffer Get(string commandBufferName = "")
    {
        if (_cmdListsCache.TryGetValue(commandBufferName, out var cmd))
        {
            uint frameIndex = frameCount % _allocationCount;
            
            _allocator[frameIndex].Reset();
            cmd.CommandList.Reset(_allocator[frameIndex]);
            
            frameCount++;
            
            cmd.CommandList.BeginEvent(commandBufferName);
            
            return cmd;
        }
        
        ID3D12GraphicsCommandList list = _device.CreateCommandList<ID3D12GraphicsCommandList>(CommandListType.Direct, _allocator[0]);
        list.Name = commandBufferName;
        _cmdListsCache[commandBufferName] = new CommandBuffer(list);
        
        return _cmdListsCache[commandBufferName];
    }

    public void Release(CommandBuffer cmd)
    {
        cmd.Release();
    }
}