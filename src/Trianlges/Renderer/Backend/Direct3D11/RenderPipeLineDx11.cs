using Trianlges.Renderer.Type;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace Trianlges.Renderer.Backend.Direct3D11;

public class RenderPipeLineDx11
{
    private readonly ProgramDx11 programe = null!;
    
    protected readonly ID3D11SamplerState SamplerState;
    
    protected readonly ID3D11RasterizerState RasterizerState;
    protected readonly ID3D11RasterizerState WireFrameRasterizerState;
    
    protected readonly ID3D11BlendState BlendState;
    protected readonly ID3D11BlendState AlphaBlendState;

    public IProgram Program => programe;
    public bool WireFrameEnable { get; set; }
    public bool BlendEnable { get; set; }

    public RenderPipeLineDx11(ID3D11Device device)
    {
        RasterizerDescription rsDesc = new(CullMode.None, FillMode.Solid);
        RasterizerState = device.CreateRasterizerState(rsDesc);

        rsDesc.FillMode = FillMode.Wireframe;
        WireFrameRasterizerState = device.CreateRasterizerState(rsDesc);

        BlendDescription blDesc = new();
        ref RenderTargetBlendDescription rtDesc = ref blDesc.RenderTarget[0];
        blDesc.AlphaToCoverageEnable = true;
        blDesc.IndependentBlendEnable = false;
        rtDesc.BlendEnable = false;
        rtDesc.RenderTargetWriteMask = ColorWriteEnable.All;
        BlendState = device.CreateBlendState(blDesc);

        blDesc.AlphaToCoverageEnable = false;
        rtDesc.BlendEnable = true;
        rtDesc.SourceBlend = Blend.SourceAlpha;
        rtDesc.DestinationBlend = Blend.DestinationAlpha;
        rtDesc.BlendOperation = BlendOperation.Add;
        rtDesc.SourceBlendAlpha = Blend.One;
        rtDesc.DestinationBlendAlpha = Blend.Zero;
        rtDesc.BlendOperationAlpha = BlendOperation.Add;
        AlphaBlendState = device.CreateBlendState(blDesc);

        SamplerDescription samDesc = new(Filter.MinPointMagMipLinear, TextureAddressMode.Wrap, 0, 1,
            ComparisonFunction.Never, 0);
        SamplerState = device.CreateSamplerState(samDesc);
    }

    public void UpDate(PipeLineDescriptor pipeLineDescriptor)
    {
        UpDateBlendState(pipeLineDescriptor.BlendOptions);
    }
    
    public void Bind(ID3D11DeviceContext context)
    {
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        
        context.RSSetState(WireFrameEnable ? WireFrameRasterizerState : RasterizerState);
        context.OMSetBlendState(BlendEnable ? AlphaBlendState : BlendState);
        context.PSSetSamplers(0, [SamplerState]);
    }

    private void UpDateBlendState(BlendOptions options)
    {
        if (options.BlendEnable)
        {
            
        }
    }
}