using System;
using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Util;

internal class PointGenerator
{
    private static readonly Random RNG = new();

    public static List<TriangulationPoint> UniformDistribution(int n, double scale)
    {
        List<TriangulationPoint> list = [];
        for (int i = 0; i < n; i++)
        {
            list.Add(new TriangulationPoint(scale * (0.5 - RNG.NextDouble()), scale * (0.5 - RNG.NextDouble())));
        }
        return list;
    }

    public static List<TriangulationPoint> UniformGrid(int n, double scale)
    {
        double num2 = scale / n;
        double num3 = 0.5 * scale;
        List<TriangulationPoint> list = [];
        for (int i = 0; i < n + 1; i++)
        {
            double num = num3 - (i * num2);
            for (int j = 0; j < n + 1; j++)
            {
                list.Add(new TriangulationPoint(num, num3 - (j * num2)));
            }
        }
        return list;
    }
}
