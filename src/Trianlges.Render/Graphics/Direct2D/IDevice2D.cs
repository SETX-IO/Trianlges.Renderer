using Vortice.DXGI;

namespace Trianlges.Render.Graphics.Direct2D;

public interface IDevice2D : IDevice
{
    IDXGISwapChain SwapChain { get; }
}