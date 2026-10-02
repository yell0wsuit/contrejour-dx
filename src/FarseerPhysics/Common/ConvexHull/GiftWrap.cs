using System.Numerics;

namespace FarseerPhysics.Common.ConvexHull
{
    public static class GiftWrap
    {
        public static Vertices GetConvexHull(Vertices vertices)
        {
            if (vertices.Count <= 3)
            {
                return vertices;
            }
            int num = 0;
            float num2 = vertices[0].X;
            for (int i = 1; i < vertices.Count; i++)
            {
                float x = vertices[i].X;
                if (x > num2 || (x == num2 && vertices[i].Y < vertices[num].Y))
                {
                    num = i;
                    num2 = x;
                }
            }
            int[] array = new int[vertices.Count];
            int num3 = 0;
            int num4 = num;
            int num5;
            do
            {
                array[num3] = num4;
                num5 = 0;
                for (int j = 1; j < vertices.Count; j++)
                {
                    if (num5 == num4)
                    {
                        num5 = j;
                        continue;
                    }
                    Vector2 a = vertices[num5] - vertices[array[num3]];
                    Vector2 b = vertices[j] - vertices[array[num3]];
                    float num6 = MathUtils.Cross(ref a, ref b);
                    if (num6 < 0f)
                    {
                        num5 = j;
                    }
                    if (num6 == 0f && b.LengthSquared() > a.LengthSquared())
                    {
                        num5 = j;
                    }
                }
                num3++;
                num4 = num5;
            }
            while (num5 != num);
            Vertices vertices2 = new(num3);
            for (int k = 0; k < num3; k++)
            {
                vertices2.Add(vertices[array[k]]);
            }
            return vertices2;
        }
    }
}
