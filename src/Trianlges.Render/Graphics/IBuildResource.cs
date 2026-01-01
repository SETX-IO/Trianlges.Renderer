using System;

namespace Trianlges.Render.Graphics;

public interface IBuildResource
{
    T Build<T>() where T : class, IBuildResource
    {
        var instnce = this as T;
        return instnce ?? throw new TypeAccessException();
    }
}