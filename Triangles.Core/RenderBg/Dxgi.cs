using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class Dxgi
{
#if DEBUG
    private const bool IsDebug = true;
#else
        private const bool IsDebug = false;   
#endif
        
    public static readonly Lazy<IDXGIFactory7> Factory = new (DXGI.CreateDXGIFactory2<IDXGIFactory7>(IsDebug));

    static Dxgi()
    {
#if DEBUG
        D3D12.D3D12GetDebugInterface<ID3D12Debug3>(out var dxDebug);
        dxDebug.EnableDebugLayer();
        dxDebug.SetEnableGPUBasedValidation(true);
        dxDebug.Dispose();
#endif
    }
}