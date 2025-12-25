using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public interface IDevice
{
    IDXGISwapChain SwapChain { get; }

    void Present()
    {
        SwapChain.Present(0, PresentFlags.None);
    }
}