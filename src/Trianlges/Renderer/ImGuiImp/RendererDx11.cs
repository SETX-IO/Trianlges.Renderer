using System.Numerics;
using Hexa.NET.ImGui;
using Trianlges.Graphics.Direct3D11;
using System.Runtime.InteropServices;
using Trianlges.Renderer.Backend.Direct3D11;
using Vortice.Direct3D11;

namespace Trianlges.Renderer.ImGuiImp;

public class RendererDx11
{
    private readonly Device3D _device;
    private BufferDx11<ImDrawVert>? _vBuffer;
    private BufferDx11<ushort>? _iBuffer;
    private BufferDx11<Matrix4x4>? _cBuffer;

    private int _vertexCount;
    private int _indexCount;
    
    public RendererDx11(Device3D device)
    {
        _device = device;
        
        ImGuiContextPtr ctx = ImGui.CreateContext();
        ImGui.SetCurrentContext(ctx);
        
        Initializer();
    }

    public void Render()
    {
        ImDrawDataPtr drawData = ImGui.GetDrawData();
        if (drawData.Textures.Size != 0)
        {
            
        }

        if (_vBuffer == null || _vertexCount < drawData.TotalVtxCount)
        {
            _vertexCount = drawData.TotalVtxCount + 5000;
            _vBuffer = _device.NewBuffer<ImDrawVert>(BindFlags.VertexBuffer, bufferSize: (uint)_vertexCount);
        }
        
        if (_iBuffer == null  || _indexCount < drawData.TotalIdxCount)
        {
            _indexCount = drawData.TotalIdxCount + 10000;
            _iBuffer = _device.NewBuffer<ushort>(BindFlags.IndexBuffer, bufferSize: (uint)_indexCount);
        }
    }
    
    private unsafe void Initializer()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.BackendRendererName = (byte*)Marshal.StringToBSTR("Trianlges.ImGuiRender");

        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasTextures;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
    }
}