using Triangles.Core.Renderer;
using Vortice.Direct3D12;

namespace Triangles.Core.RenderBg;

public partial class Device : IDevice
{
    private readonly List<RootParameter1> _rootSignatureParameters = new();
    private readonly List<StaticSamplerDescription> _samplerDescriptions = new();
    
    public ReadOnlyMemory<RenderTargetTexture> CreateRenderTargetViews(uint bufferCount, Func<int, ID3D12Resource> backBufferCallBack)
    {
            
        RenderTargetTexture[] resources = new RenderTargetTexture[bufferCount];

        uint rtvDescSize = RtvDescriptorSize;
        
        DescriptionHeap heap = DescriptionHeapPool.GetHeap(DescriptionHeapType.Rtv);
        CpuDescriptorHandle rtvHandel = heap.CpuHandle;

        for (int i = 0; i < resources.Length; i++)
        {
            ID3D12Resource backBuffer = backBufferCallBack(i);
                
            _device.CreateRenderTargetView(backBuffer, null, rtvHandel);
            resources[i] = new RenderTargetTexture(backBuffer, rtvHandel, this);

            rtvHandel.Offset(1, rtvDescSize);
        }
            
        return resources;
    }

    public ID3D12Fence CreateFence(ulong initialValue = 0UL, FenceFlags flags = FenceFlags.None)
    {
        return _device.CreateFence(initialValue, flags);
    }

    public ID3D12PipelineState CreatePipLine(GraphicsPipelineStateDescription pipeLineInfo) =>
        _device.CreateGraphicsPipelineState(pipeLineInfo);
        
    public CommandBufferPool CreateCommandBufferPool(uint allocationCount)
    {
        CommandBufferPool pool = new CommandBufferPool(_device, allocationCount);
        return pool;
    }

    public Device AddRootSignatureParameter(ShaderVisibility shaderVisibility, DescriptorRangeType type, uint registerSlot)
    {
        DescriptorRange1 tableRange = new DescriptorRange1(type, 1, registerSlot);
        RootParameter1 parameter = new RootParameter1(new RootDescriptorTable1(tableRange), shaderVisibility);
        
        _rootSignatureParameters.Add(parameter);

        return this;
    }

    public Device AddSamplerDesc()
    {
        StaticSamplerDescription samplerInfo = new StaticSamplerDescription(0)
        {
            ShaderVisibility = ShaderVisibility.Pixel
        };

        _samplerDescriptions.Add(samplerInfo);
        
        return this;
    }

    public void SerializedRootSignature(RootSignatureFlags signatureFlags = RootSignatureFlags.AllowInputAssemblerInputLayout)
    {
        RootSignatureDescription1 signatureInfo = new(signatureFlags, _rootSignatureParameters.ToArray(), _samplerDescriptions.ToArray());
        
        RootSignature = _device.CreateRootSignature(signatureInfo);
    }
}