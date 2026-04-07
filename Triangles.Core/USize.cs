using System.Drawing;

namespace Triangles.Core;

public struct USize : IEquatable<USize>
{
    public static USize Empty = new(0, 0); 
    
    public uint Width;
    public uint Height;

    public USize(uint width, uint height)
    {
        Width = width;
        Height = height;
    }
    
    public static implicit operator USize(Size size)
    {
        USize uSize = new USize((uint)size.Width, (uint)size.Height);
        return uSize;
    }

    public bool Equals(USize other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        return obj is USize other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(USize left, USize right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(USize left, USize right)
    {
        return !left.Equals(right);
    }
}