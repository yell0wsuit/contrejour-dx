using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common
{
    public struct Mat22
    {
        public Vector2 ex;

        public Vector2 ey;

        public Mat22 Inverse
        {
            get
            {
                float x = ex.X;
                float x2 = ey.X;
                float y = ex.Y;
                float y2 = ey.Y;
                float num = (x * y2) - (x2 * y);
                if (num != 0f)
                {
                    num = 1f / num;
                }
                return new Mat22
                {
                    ex =
                    {
                        X = num * y2,
                        Y = (0f - num) * y
                    },
                    ey =
                    {
                        X = (0f - num) * x2,
                        Y = num * x
                    }
                };
            }
        }

        public Mat22(Vector2 c1, Vector2 c2)
        {
            ex = c1;
            ey = c2;
        }

        public Mat22(float a11, float a12, float a21, float a22)
        {
            ex = new Vector2(a11, a21);
            ey = new Vector2(a12, a22);
        }

        public void Set(Vector2 c1, Vector2 c2)
        {
            ex = c1;
            ey = c2;
        }

        public void SetIdentity()
        {
            ex.X = 1f;
            ey.X = 0f;
            ex.Y = 0f;
            ey.Y = 1f;
        }

        public void SetZero()
        {
            ex.X = 0f;
            ey.X = 0f;
            ex.Y = 0f;
            ey.Y = 0f;
        }

        public readonly Vector2 Solve(Vector2 b)
        {
            float x = ex.X;
            float x2 = ey.X;
            float y = ex.Y;
            float y2 = ey.Y;
            float num = (x * y2) - (x2 * y);
            if (num != 0f)
            {
                num = 1f / num;
            }
            return new Vector2(num * ((y2 * b.X) - (x2 * b.Y)), num * ((x * b.Y) - (y * b.X)));
        }

        public static void Add(ref Mat22 A, ref Mat22 B, out Mat22 R)
        {
            R.ex = A.ex + B.ex;
            R.ey = A.ey + B.ey;
        }
    }
}
