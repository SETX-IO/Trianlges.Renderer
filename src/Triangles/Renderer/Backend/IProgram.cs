using Vortice.Direct3D11;

namespace Triangles.Renderer.Backend;

public interface IProgram
{
    public void Bind(ID3D11DeviceContext context);
}