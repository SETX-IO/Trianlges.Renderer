using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.DXGI;
using Vortice.Mathematics;
using AlphaMode = Vortice.DCommon.AlphaMode;

namespace Trianlges.Graphics.Direct2D;

public class Renderer : IRenderer
{
    private readonly IDevice2D _device2D;
    // private readonly IDXGIFactory1 _giFactory;
    private readonly ID2D1Factory _d2DFactory;
    private readonly IDWriteFactory _writeFactory;

    private ID2D1RenderTarget? _renderTarget;
    private ID2D1SolidColorBrush _textColor = null!;
    private IDWriteTextFormat _textFont  = null!;
    
    public Renderer(IDevice2D device)
    {
        _device2D = device;
        // _giFactory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
        _d2DFactory = D2D1.D2D1CreateFactory<ID2D1Factory>();
        _writeFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();
    }

    public void Init()
    {
        ConfigRenderTarget();

        _textFont = _writeFactory.CreateTextFormat("Comic Sans MS", 20);
        _textColor = _renderTarget!.CreateSolidColorBrush(new Color(255, 255,255));
    }

    private void ConfigRenderTarget()
    {
        var sw = _device2D.SwapChain;
        var backBuffer = sw.GetBuffer<IDXGISurface>(0);

        var properties = new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied));
        _renderTarget = _d2DFactory.CreateDxgiSurfaceRenderTarget(backBuffer, properties);
    }
    
    public void Updata()
    {
        
    }

    public void Render()
    {
        if (_renderTarget == null) return;
        
        _renderTarget.BeginDraw();
        
        _renderTarget.DrawText("Hello Direct 2D", _textFont, new Rect(200, 20), _textColor);
        
        _renderTarget.EndDraw()
            .CheckError();
    }
}