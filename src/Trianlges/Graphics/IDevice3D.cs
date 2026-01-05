using Vortice.Direct3D11;

namespace Trianlges.Graphics;

public interface IDevice3D : IDevice
{
    ID3D11Device Device { get; }
    ID3D11DeviceContext DContext { get; }
    
    ID3D11RenderTargetView? RenderTarget { get; }
    ID3D11DepthStencilView? DepthStencil { get; }
}