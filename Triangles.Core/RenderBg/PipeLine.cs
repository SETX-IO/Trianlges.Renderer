using Triangles.Core.Renderer;
using Vortice.D3DCompiler;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class PipeLine : IPipeLine
{
    private CachedPipelineState PipeLineCache;
    
    private readonly Device _device;
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
            cbuffer ViewProj : register(b0)
            {
                matrix SRPMatrix;
            }
            
            Texture2D g_texture : register(t0);
            SamplerState g_sampler : register(s0);
            
            struct Attributes
            {
                float3 position : POSITION;
                float2 uv : TEXCOORD;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD;
            };

            Varyings vert(Attributes In)
            {
                Varyings Out;
                
                Out.position = float4(In.position, 1.0f);
                Out.position = mul(Out.position, SRPMatrix);
                
                Out.uv = In.uv;

                return Out;
            }

            float4 frag(Varyings In) : SV_Target
            {
                return g_texture.Sample(g_sampler, In.uv);
            }            
            """;

        var vertShader = Compiler.Compile(shader, "vert", "VS", "vs_5_0");
        var fragShader = Compiler.Compile(shader, "frag", "PS", "ps_5_0");

        InputElementDescription[] inputs =
        [
            new("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            new("TEXCOORD", 0, Format.R32G32_Float, 12, 0),
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
            RenderTargetFormats = [Format.R8G8B8A8_UNorm],
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

    public void Dispose()
    {
        PipelineState.Dispose();
    }
}