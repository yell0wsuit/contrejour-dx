using System;

using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public static class Distance
{
    [ThreadStatic]
    public static int GJKCalls;

    [ThreadStatic]
    public static int GJKIters;

    [ThreadStatic]
    public static int GJKMaxIters;

    public static void ComputeDistance(out DistanceOutput output, out SimplexCache cache, DistanceInput input)
    {
        cache = default;
        GJKCalls++;
        Simplex simplex = default;
        simplex.ReadCache(ref cache, input.ProxyA, ref input.TransformA, input.ProxyB, ref input.TransformB);
        FixedArray3<int> fixedArray = default;
        FixedArray3<int> fixedArray2 = default;
        int num = 0;
        while (num < 20)
        {
            int count = simplex.Count;
            for (int i = 0; i < count; i++)
            {
                fixedArray[i] = simplex.V[i].IndexA;
                fixedArray2[i] = simplex.V[i].IndexB;
            }
            switch (simplex.Count)
            {
                case 2:
                    simplex.Solve2();
                    break;
                case 3:
                    simplex.Solve3();
                    break;
            }
            if (simplex.Count == 3)
            {
                break;
            }
            Vector2 searchDirection = simplex.GetSearchDirection();
            if (searchDirection.LengthSquared() < 1.4210855E-14f)
            {
                break;
            }
            SimplexVertex value = simplex.V[simplex.Count];
            value.IndexA = input.ProxyA.GetSupport(MathUtils.MulT(input.TransformA.q, -searchDirection));
            value.WA = MathUtils.Mul(ref input.TransformA, input.ProxyA.Vertices[value.IndexA]);
            value.IndexB = input.ProxyB.GetSupport(MathUtils.MulT(input.TransformB.q, searchDirection));
            value.WB = MathUtils.Mul(ref input.TransformB, input.ProxyB.Vertices[value.IndexB]);
            value.W = value.WB - value.WA;
            simplex.V[simplex.Count] = value;
            num++;
            GJKIters++;
            bool flag = false;
            for (int j = 0; j < count; j++)
            {
                if (value.IndexA == fixedArray[j] && value.IndexB == fixedArray2[j])
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                break;
            }
            simplex.Count++;
        }
        GJKMaxIters = Math.Max(GJKMaxIters, num);
        simplex.GetWitnessPoints(out output.PointA, out output.PointB);
        output.Distance = (output.PointA - output.PointB).Length();
        output.Iterations = num;
        simplex.WriteCache(ref cache);
        if (input.UseRadii)
        {
            float radius = input.ProxyA.Radius;
            float radius2 = input.ProxyB.Radius;
            if (output.Distance > radius + radius2 && output.Distance > 1.1920929E-07f)
            {
                output.Distance -= radius + radius2;
                Vector2 vector = output.PointB - output.PointA;
                vector.Normalize();
                output.PointA += radius * vector;
                output.PointB -= radius2 * vector;
            }
            else
            {
                output.PointB = output.PointA = 0.5f * (output.PointA + output.PointB);
                output.Distance = 0f;
            }
        }
    }
}
