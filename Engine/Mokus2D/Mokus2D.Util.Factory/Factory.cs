using System;

namespace Mokus2D.Util.Factory;

public class Factory<T> : IFactory<T>
{
    public T New()
    {
        return Activator.CreateInstance<T>();
    }
}
