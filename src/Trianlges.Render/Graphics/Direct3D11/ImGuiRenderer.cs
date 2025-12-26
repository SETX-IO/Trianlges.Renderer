using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Trianlges.Render.Graphics.Direct3D11;

public class ImGuiRenderer : IRenderer
{
    private readonly IDevice3D _device;
    private readonly Mesh _imGuiMesh;
    
    private ID3D11Buffer? _contextBuffer;

    private ID3D11Texture2D _fontTexture;
    private ID3D11SamplerState _fontSampler;

    private Dictionary<IntPtr, ID3D11ShaderResourceView> _textureCache = new();
    
    static ImGuiRenderer()
    {
        var guiContxet = ImGui.CreateContext();
        ImGui.SetCurrentContext(guiContxet);
    }
    
    public ImGuiRenderer(IDevice3D device)
    {
        _device = device;
        _imGuiMesh = new Mesh();

        var swDesc = _device.SwapChain.Description;
        var io = ImGui.GetIO();

        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.DisplaySize = new Vector2(swDesc.BufferDescription.Width, swDesc.BufferDescription.Height);
        
        Init(_device.Device);
    }

    public unsafe void Updata()
    {
        ImGui.NewFrame();
        ImGui.Render();
        
        var drawData = ImGui.GetDrawData();
        var ctx = _device.DContext;
        
        var vertexMap = ctx.Map(_imGuiMesh.VertextBuffer, 0, MapMode.WriteDiscard);
        var indexMap = ctx.Map(_imGuiMesh.IndexBuffer!, 0, MapMode.WriteDiscard);
        var vtxResourcePtr = (ImDrawVert*)vertexMap.DataPointer;
        var idxResourcePtr = (uint*)indexMap.DataPointer;
        
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            var cmd = drawData.CmdLists[i];
            
            var vtxSize = cmd.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
            Buffer.MemoryCopy((void*)cmd.VtxBuffer.Data, vtxResourcePtr, vtxSize, vtxSize);
            
            var idxSize = cmd.IdxBuffer.Size * Unsafe.SizeOf<uint>();
            Buffer.MemoryCopy((void*)cmd.IdxBuffer.Data, idxResourcePtr, idxSize, idxSize);
            
            vtxResourcePtr += cmd.VtxBuffer.Size;
            idxResourcePtr += cmd.IdxBuffer.Size;
        }
        
        ctx.Unmap(_imGuiMesh.IndexBuffer!);
        ctx.Unmap(_imGuiMesh.VertextBuffer);

        var constantMap = ctx.Map(_contextBuffer, MapMode.WriteDiscard);
        var span = constantMap.AsSpan<float>(16 * 4);
        float L = drawData.DisplayPos.X;
        float R = drawData.DisplayPos.X + drawData.DisplaySize.X;
        float T = drawData.DisplayPos.Y;
        float B = drawData.DisplayPos.Y + drawData.DisplaySize.Y;
        float[] mvp =
        {
            2.0f/(R-L),   0.0f,           0.0f,       0.0f,
            0.0f,         2.0f/(T-B),     0.0f,       0.0f,
            0.0f,         0.0f,           0.5f,       0.0f,
            (R+L)/(L-R),  (T+B)/(B-T),    0.5f,       1.0f,
        };
        mvp.CopyTo(span);
        
        ctx.Unmap(_contextBuffer);
        
        ctx.PSSetSampler(0, _fontSampler);
        
        int global_idx_offset = 0;
        int global_vtx_offset = 0;
        Vector2 clip_off = drawData.DisplayPos;
        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            var cmdList = drawData.CmdLists[n];
            for (int i = 0; i < cmdList.CmdBuffer.Size; i++)
            {
                var cmd = cmdList.CmdBuffer[i];
                if (cmd.UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException("user callbacks not implemented");
                }
                else
                {
                    var rect = new Rect((int)(cmd.ClipRect.X - clip_off.X), (int)(cmd.ClipRect.Y - clip_off.Y), (int)(cmd.ClipRect.Z - clip_off.X), (int)(cmd.ClipRect.W - clip_off.Y));
                    ctx.RSSetScissorRects([rect]);

                    _textureCache.TryGetValue(cmd.TextureId, out var texture);
                    if (texture != null)
                        ctx.PSSetShaderResources(0, [texture]);

                    ctx.DrawIndexed(cmd.ElemCount, (uint)(cmd.IdxOffset + global_idx_offset), (int)(cmd.VtxOffset + global_vtx_offset));
                }
            }
            global_idx_offset += cmdList.IdxBuffer.Size;
            global_vtx_offset += cmdList.VtxBuffer.Size;
        }
    }

    public void Render()
    {

    }

    private void Init(ID3D11Device device)
    {
        var vertexInput = VertexInputElement.GetVertextElements(VertextType.Position2, VertextType.Color4, VertextType.Uv);
        var progame = ShaderProgame
            .Create(device)
            .Complier("Assets/ImGuiShader.hlsl")
            .ConfigInput(vertexInput)
            .Build();

        var cBufferDesc = new BufferDescription(16 * 4, BindFlags.ConstantBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _contextBuffer = _device.Device.CreateBuffer(cBufferDesc);
            
        _device.DContext.VSSetConstantBuffers(1, [_contextBuffer]);
        
        InitFonts();
        
        ImGui.NewFrame();
        ImGui.Render();
        
        var drawData = ImGui.GetDrawData();
        
        var vBufferDesc = new BufferDescription((uint)((drawData.TotalVtxCount + 5000) * Unsafe.SizeOf<ImDrawVert>()),
            BindFlags.VertexBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);
        var vBuffer = device.CreateBuffer(vBufferDesc);
        
        var iBufferDesc = new BufferDescription((uint)((drawData.TotalIdxCount + 10000) * Unsafe.SizeOf<uint>()),
            BindFlags.VertexBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);
        var iBuffer = device.CreateBuffer(iBufferDesc);
        
        _imGuiMesh.Init(vBuffer, iBuffer,  progame);
    }

    private unsafe void InitFonts()
    {
        var device = _device.Device;
        var io = ImGui.GetIO();
        
        io.Fonts.GetTexDataAsRGBA32(out byte* pixel, out var width, out var height);

        var fontTextureDesc = new Texture2DDescription(Format.R8G8B8A8_UNorm, (uint)width, (uint)height, 1, 1);
        var textureData = new SubresourceData(pixel, fontTextureDesc.Width * 4);

        _fontTexture = device.CreateTexture2D(fontTextureDesc, textureData);
        var sResourceDesc = new ShaderResourceViewDescription(_fontTexture, ShaderResourceViewDimension.Texture2D, Format.R8G8B8A8_UNorm);

        var fontTextureResource = device.CreateShaderResourceView(_fontTexture, sResourceDesc);
        _textureCache.TryAdd(fontTextureResource.NativePointer, fontTextureResource);
        io.Fonts.TexID = fontTextureResource.NativePointer;

        var samplerDesc = new SamplerDescription(Filter.MinMagMipLinear, TextureAddressMode.Wrap, comparisonFunc: ComparisonFunction.Always, minLOD: 0, maxLOD:0);
        _fontSampler = device.CreateSamplerState(samplerDesc);
    }
}