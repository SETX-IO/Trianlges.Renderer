using Trianlges.Render.Graphics.Direct3D11;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public abstract class DrawElement
{
    protected ID3D11Buffer _vBuffer;
    protected ID3D11Buffer? _iBuffer;
    protected ID3D11PixelShader? _pShader;
    protected ID3D11VertexShader? _vShader;
    protected ID3D11InputLayout? _vShaderLayout;

    public virtual void Updata()
    {
    }

    public virtual void Render(D3DDevice device)
    {
        var context = device.DContext;
        var stride = Vertex.Size;
        uint offset = 0;

        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

        context.VSSetShader(_vShader);
        context.PSSetShader(_pShader);
        
        context.IASetVertexBuffers(0, 1, [_vBuffer], [stride], [offset]);
        
        context.IASetInputLayout(_vShaderLayout);
        if (_iBuffer != null) context.IASetIndexBuffer(_iBuffer, Format.R32_UInt, 0);
    }
}