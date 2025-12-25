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

public struct ConstantBufferData(Matrix4x4 world, Matrix4x4 view, Matrix4x4 proj)
{
    public Matrix4x4 Module = world;
    public Matrix4x4 View = view;
    public Matrix4x4 Proj = proj;


    public static readonly uint Size = (uint)Unsafe.SizeOf<ConstantBufferData>();
}

public enum VertextType
{
    Positon,
    Color3,
    Color4,
    Normal
}

public struct VertexInputElement
{
    private static uint Offset;
    
    public static InputElementDescription[] GetVertextElements(params VertextType[] element)
    {
        var descs = new List<InputElementDescription>();
        foreach (var vertextType in element)
        {
            
            switch (vertextType)
            {
                case VertextType.Positon:
                    descs.Add(Position);
                    break;
                case VertextType.Color3:
                    descs.Add(Color3);
                    break;
                case VertextType.Color4:
                    descs.Add(Color4);
                    break;
                case VertextType.Normal:
                    descs.Add(Normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Offset = 0;
        
        return descs.ToArray();
    }
    
    public static InputElementDescription Position
    {
        get
        {
            var element = new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, Offset, 0);
            Offset += 12;

            return element;
        }
    }
    
    public static InputElementDescription Color3
    {
        get
        {
            var element = new InputElementDescription("COLOR", 0, Format.R32G32B32_Float, Offset, 0);
            Offset += 12;

            return element;
        }
    }
    
    public static InputElementDescription Color4
    {
        get
        {
            var element = new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, Offset);
            Offset += 16;

            return element;
        }
    }

    public static InputElementDescription Normal
    {
        get
        {
            var element = new InputElementDescription("NORMAL", 0, Format.R32G32B32_Float, Offset);
            Offset += 12;

            return element;
        }
    }
}