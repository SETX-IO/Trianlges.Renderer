using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Renderer;

public struct Vertex
{
    public static readonly InputElementDescription[] VertexInputLayout =
        VertexInputElement.GetVertextElements(VertexType.Position, VertexType.Color3, VertexType.Uv);
    
    public Vector3 Position;
    public Vector3 Color;
    public Vector2 Uv;
    
    public Vertex(Vector3 position, Vector2 uv)
    {
        Position = position;
        Color = Vector3.One;
        Uv = uv;
    }
    
    public Vertex(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
        Uv = Vector2.Zero;
    }
    
    public Vertex(Vector3 position)
    {
        Position = position;
        Color = Vector3.One;
        Uv = Vector2.One;
    }
    
    public static readonly uint Size = (uint)Unsafe.SizeOf<Vertex>();
}

public struct ConstantBufferData(Matrix4x4 view, Matrix4x4 proj)
{
    public Matrix4x4 View = view;
    public Matrix4x4 Proj = proj;

    public static readonly uint Size = (uint)Unsafe.SizeOf<ConstantBufferData>();
}

public enum VertexType
{
    Position,
    Position2,
    Color3,
    Color4,
    Uv,
    Normal
}

public struct VertexInputElement
{
    private static uint _offset;
    
    public static InputElementDescription[] GetVertextElements(params VertexType[] elements)
    {
        var descs = elements.Select(element => element switch
        {
            VertexType.Position => Position,
            VertexType.Position2 => Position2,
            VertexType.Color3 => Color3,
            VertexType.Color4 => Color4,
            VertexType.Uv => Uv,
            VertexType.Normal => Normal,
            _ => throw new ArgumentOutOfRangeException()
        });
        _offset = 0;
        
        return descs.ToArray();
    }

    private const uint Float32 = 4;
    private const uint Float64 = Float32 * 2;
    private const uint Float96 = Float32 * 3;
    private const uint Float128 = Float32 * 4;

    private static InputElementDescription Position
    {
        get
        {
            var element = new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, _offset, 0);
            _offset += Float96;

            return element;
        }
    }
    
    private static InputElementDescription Position2
    {
        get
        {
            var element = new InputElementDescription("POSITION", 0, Format.R32G32_Float, _offset, 0);
            _offset += Float64;

            return element;
        }
    }
    
    private static InputElementDescription Color3
    {
        get
        {
            var element = new InputElementDescription("COLOR", 0, Format.R32G32B32_Float, _offset, 0);
            _offset += Float96;

            return element;
        }
    }
    
    private static InputElementDescription Color4
    {
        get
        {
            var element = new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, _offset, 0);
            _offset += Float128;

            return element;
        }
    }

    private static InputElementDescription Uv
    {
        get
        {
            var element = new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, _offset, 0);
            _offset += Float64;

            return element;
        }
    }
    
    private static InputElementDescription Normal
    {
        get
        {
            var element = new InputElementDescription("NORMAL", 0, Format.R32G32B32_Float, _offset, 0);
            _offset += Float96;

            return element;
        }
    }
}