using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Trianlges.Render.Graphics;

public struct Vertex(Vector3 position, Vector3 color)
{
    public Vector3 Position = position;
    public Vector3 Color = color;

    public static readonly uint Size = (uint)Unsafe.SizeOf<Vertex>();
}

public struct VertexUv(Vector3 position, Vector2 uv)
{
    public Vector3 Position = position;
    public Vector2 Uv = uv;

    public static readonly uint Size = (uint)Unsafe.SizeOf<VertexUv>();
}

public struct ConstantBufferData(Matrix4x4 world, Matrix4x4 view, Matrix4x4 proj)
{
    public Matrix4x4 Module = world;
    public Matrix4x4 View = view;
    public Matrix4x4 Proj = proj;


    public static readonly uint Size = (uint)Unsafe.SizeOf<ConstantBufferData>();
}

public enum VertextType
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
    
    public static InputElementDescription[] GetVertextElements(params VertextType[] element)
    {
        var descs = new List<InputElementDescription>();
        foreach (var vertextType in element)
        {
            var desc = vertextType switch
            {
                VertextType.Position => Position,
                VertextType.Position2 => Position2,
                VertextType.Color3 => Color3,
                VertextType.Color4 => Color4,
                VertextType.Uv => Uv,
                VertextType.Normal => Normal,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            descs.Add(desc);
        }

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