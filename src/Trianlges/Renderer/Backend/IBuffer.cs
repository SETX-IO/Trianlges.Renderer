using Vortice.Direct3D11;

namespace Trianlges.Renderer.Backend;

public interface IBuffer<in T> where T : unmanaged
{
    void Bind(ID3D11DeviceContext context, uint slot = 0);
    void Update(ID3D11DeviceContext context, T[] data, uint slot = 0, ShaderType type = ShaderType.Vertex);
}