using Vortice.D3DCompiler;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class PipeLine
{
    private CachedPipelineState PipeLineCache;
    
    private Device _device;
    private readonly GraphicsPipelineStateDescription _pipeLineInfo;
    
    public ID3D12PipelineState PipelineState;

    public CullMode CullMode
    {
        get => _pipeLineInfo.RasterizerState.CullMode;
        set
        {
            var rasterizerInfo = _pipeLineInfo.RasterizerState;

            if (rasterizerInfo.CullMode.Equals(value)) return;
            
            rasterizerInfo.CullMode = value;
            _pipeLineInfo.RasterizerState = rasterizerInfo;
            
            ReCreatePipeLine();
        }
    }

    public FillMode FillMode
    {
        get => _pipeLineInfo.RasterizerState.FillMode;
        set
        {
            var rasterizerInfo = _pipeLineInfo.RasterizerState;

            if (rasterizerInfo.FillMode.Equals(value)) return;
            
            rasterizerInfo.FillMode = value;
            _pipeLineInfo.RasterizerState = rasterizerInfo;
            
            ReCreatePipeLine();
        }
    }
    
    
    public PipeLine(Device device)
    {
        _device = device;
        
        string shader =
            """
            struct Attributes
            {
                float3 position : POSITION;
                float3 color : COLOR0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float4 color : COLOR0;
            };

            Varyings vert(Attributes In)
            {
                Varyings Out;
                
                Out.position = float4(In.position, 1.0f);
                Out.color = float4(In.color, 1.0f);

                return Out;
            }

            float4 frag(Varyings In) : SV_Target
            {
                return In.color;
            }            
            """;

        var vertShader = Compiler.Compile(shader, "vert", "VS", "vs_5_0");
        var fragShader = Compiler.Compile(shader, "frag", "PS", "ps_5_0");

        InputElementDescription[] inputs =
        [
            new("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            new("COLOR", 0, Format.R32G32B32_Float, 12, 0),
        ];

        _pipeLineInfo = new GraphicsPipelineStateDescription
        {
            InputLayout = inputs,
            RootSignature = device.RootSignature,
            VertexShader = vertShader,
            PixelShader = fragShader,
            PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
            RasterizerState = RasterizerDescription.CullCounterClockwise,
            BlendState = BlendDescription.Opaque,
            DepthStencilState = DepthStencilDescription.Default,
            RenderTargetFormats = [Format.B8G8R8A8_UNorm],
            DepthStencilFormat = Format.Unknown,
            SampleDescription = SampleDescription.Default,
        };
        
        PipelineState = _device.CreatePipLine(_pipeLineInfo);

        PipeLineCache = new CachedPipelineState
        {
            CachedBlob = PipelineState.CachedBlob.BufferPointer,
            CachedBlobSizeInBytes = PipelineState.CachedBlob.BufferSize
        };

        _pipeLineInfo.CachedPSO = PipeLineCache;
    }

    private void ReCreatePipeLine()
    {
        PipelineState = _device.CreatePipLine(_pipeLineInfo);
    }

    public void ApplyPipeLine(CommandBuffer commandBuffer)
    {
        commandBuffer.CommandList.SetPipelineState(PipelineState);
        
        commandBuffer.CommandList.SetGraphicsRootSignature(_pipeLineInfo.RootSignature);
    }
}