using ImGuiNET;

namespace Trianlges.Render.Graphics.Direct3D11;

public class ImGuiRenderer : IRenderer
{
    private readonly IDevice3D _device;
    private readonly Mesh _drawData;

    static ImGuiRenderer()
    {
        var guiContxet = ImGui.CreateContext();
        ImGui.SetCurrentContext(guiContxet);
    }
    
    public ImGuiRenderer(IDevice3D device)
    {
        _device = device;
        
        _drawData = new Mesh();
        _drawData.Init(_device);
    }

    public void Updata()
    {
        
    }

    public void Render()
    {
        var drawData = ImGui.GetDrawData();
        // _drawData.Render(_device); 
        
        // ImGui.ShowDemoWindow();
    }
}