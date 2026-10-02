using System.Numerics;

namespace FarseerPhysics.Common
{
    public struct Transform(ref Vector2 position, ref Rot rotation)
    {
        public Vector2 p = position;

        public Rot q = rotation;

        public void SetIdentity()
        {
            p = Vector2.Zero;
            q.SetIdentity();
        }

        public void Set(Vector2 position, float angle)
        {
            p = position;
            q.Set(angle);
        }
    }
}
