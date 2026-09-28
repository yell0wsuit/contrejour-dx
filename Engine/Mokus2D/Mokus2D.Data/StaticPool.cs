namespace Mokus2D.Data;

public static class StaticPool<T> where T : new()
{
    private static readonly Pool<T> Pool = new Pool<T>(() => new T());

    public static T New()
    {
        return Pool.New();
    }

    public static void Free(T item)
    {
        Pool.Free(item);
    }

    public static void Clear()
    {
        Pool.Clear();
    }
}
