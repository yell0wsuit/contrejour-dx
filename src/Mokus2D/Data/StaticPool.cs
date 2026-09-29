namespace Mokus2D.Data
{
    public static class StaticPool
    {
        public static T New<T>() where T : new()
        {
            return Holder<T>.Pool.New();
        }

        public static void Free<T>(T item) where T : new()
        {
            Holder<T>.Pool.Free(item);
        }

        public static void Clear<T>() where T : new()
        {
            Holder<T>.Pool.Clear();
        }

        // One pool per type, created the first time that type is pooled.
        private static class Holder<T> where T : new()
        {
            public static readonly Pool<T> Pool = new(() => new T());
        }
    }
}
