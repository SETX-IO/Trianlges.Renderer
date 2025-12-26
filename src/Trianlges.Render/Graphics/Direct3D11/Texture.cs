using Vortice.Direct3D11;

namespace Trianlges.Render.Graphics.Direct3D11;

public class Texture
{
    private ID3D11ShaderResourceView _texture = null!;
    private ID3D11SamplerState _sampler = null!;
    
    public Texture()
    {
        
    }
}