using System.Numerics;
using Vortice.Direct3D11;

namespace Triangles.Renderer;

public class RenderCommand
{
    public enum Type
    {
        UnknownCommand,
        QuadCommand,
        CustomCommand,
        GroupCommand,
        MeshCommand,
        TrianglesCommand,
        CallBackCommand,
        CaptureScreenCommand
    }
    
    protected ID3D11DeviceContext Context;
    private Type _type = Type.UnknownCommand;

    public RenderCommand(float globalZOrder, Matrix4x4 transform, uint flag)
    {
        
    }
}