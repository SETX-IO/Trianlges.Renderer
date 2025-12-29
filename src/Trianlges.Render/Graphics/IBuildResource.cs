using System;

namespace Trianlges.Render.Graphics;

public interface IBuildResource
{
    T Build<T>() where T : class, IBuildResource
    {
        throw new NotImplementedException();
    }
}