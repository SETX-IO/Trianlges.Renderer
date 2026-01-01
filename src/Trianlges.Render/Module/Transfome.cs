using System.Numerics;

namespace Trianlges.Render.Module;

public class Transfome
{
    public Transfome() {}
    public Transfome(Vector3 scale, Vector3 rotation, Vector3 position)
    {
        Scale = scale;
        Rotation = rotation;
        Position = position;
    }

    public Vector3 Scale { get; set; } = Vector3.One;
    public Vector3 Rotation { get; set; } = Vector3.Zero;
    public Vector3 Position { get; set; } = Vector3.Zero;

    private Matrix4x4 ScaleMat => Matrix4x4.CreateScale(Scale);
    private Matrix4x4 RoattionMat
    {
        get
        {
            var axisX = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
            var axisY = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
            var axisZ = Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z);

            var rightAixs = axisX * axisY * axisZ;
            return rightAixs;
        }
    }
    private Matrix4x4 PostionMat => Matrix4x4.CreateTranslation(Position);
    public Matrix4x4 WorldMat => Matrix4x4.Transpose(ScaleMat * RoattionMat * PostionMat);
    


    public void SetScale(float x, float y, float z) => Scale = Scale with { X = x, Y = y, Z = z};
    public void SetRotation(float x, float y, float z) => Rotation = Rotation with { X = x, Y = y, Z = z};
    public void SetPosition(float x, float y, float z) => Position = Position with { X = x, Y = y, Z = z};
}