using System.Numerics;
using Vortice.Mathematics;

namespace Trianlges.Graphics;

public class Camera
{
    private int _fov;
    private Vector3 _pos;
    private static Color _clearColor = new(0, 50, 100);

    public Camera(Vector3 pos, int fov = 45)
    {
        _fov = fov;
        _pos = pos;

        Updata(_pos, Vector3.Zero, _fov);
    }

    public int Fov
    {
        get => _fov;
        set
        {
            if (Equals(value, _fov)) return;

            _fov = value;
            Updata(_pos, Vector3.Zero, _fov);
        }
    }

    public Vector3 Position
    {
        get => _pos;
        set
        {
            if (Equals(value, _pos)) return;

            _pos = value;
            Updata(_pos, Vector3.Zero, _fov);
        }
    }

    public static Color ClearColor
    {
        get => _clearColor;
        set
        {
            if (Equals(value, _clearColor)) return;

            _clearColor = value;
        }
    }

    public Matrix4x4 View { get; private set; }
    public Matrix4x4 Proj { get; private set; }

    public void Updata() => Updata(_pos, Vector3.Zero, _fov);
    
    private void Updata(Vector3 pos, Vector3 at, float fov)
    {
        View = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(float.DegreesToRadians(fov), Window.AspectRatio, 1,
            100);
        Proj = Matrix4x4.CreateLookAtLeftHanded(pos, at, Vector3.UnitY);
    }
}