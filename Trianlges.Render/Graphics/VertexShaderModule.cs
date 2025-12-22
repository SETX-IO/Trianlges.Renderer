using System.Numerics;
using System.Runtime.CompilerServices;

namespace Trianlges.Render.Graphics;

public struct Vertex(Vector3 position, Vector3 color)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
    
    public static readonly uint Size = (uint)Unsafe.SizeOf<Vertex>();
}

public struct ConstantBufferData(Matrix4x4 mvp)
{
    public Matrix4x4 Mvp = mvp;
    // public Matrix4x4 View = view;
    // public Matrix4x4 Module = Matrix4x4.Identity;


    public static readonly uint Size = (uint)Unsafe.SizeOf<ConstantBufferData>();
}