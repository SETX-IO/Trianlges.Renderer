using ImGuiNET;

namespace Trianlges.Render.Graphics.Direct3D11;

public class ImGuiRenderer : IRenderer
{
    private readonly IDevice3D _device;
    private Mesh? _drawData;
    
    public ImGuiRenderer(IDevice3D device)
    {
        _device = device;
    }

    public void Updata()
    {
        
    }

    public void Render()
    {
        var drawData = ImGui.GetDrawData();

        if (_drawData == null)
        {
            _drawData = new Mesh();
            _drawData.Init(_device);
        }
        
        
        
        // ImGui.ShowDemoWindow();
    }
}