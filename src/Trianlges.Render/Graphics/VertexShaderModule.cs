using System.Numerics;
using System.Runtime.CompilerServices;

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