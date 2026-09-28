using System;

using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Util;

internal sealed class PolygonGenerator
{
    private static readonly Random RNG = new();

    private static double PI_2 = Math.PI * 2.0;

    public static Polygon.Polygon RandomCircleSweep(double scale, int vertexCount)
    {
        double num = scale / 4.0;
        PolygonPoint[] array = new PolygonPoint[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            do
            {
                num = (i % 250 == 0) ? (num + (scale / 2.0 * (0.5 - RNG.NextDouble()))) : ((i % 50 != 0) ? (num + (25.0 * scale / vertexCount * (0.5 - RNG.NextDouble()))) : (num + (scale / 5.0 * (0.5 - RNG.NextDouble()))));
                num = (num > scale / 2.0) ? (scale / 2.0) : num;
                num = (num < scale / 10.0) ? (scale / 10.0) : num;
            }
            while (num < scale / 10.0 || num > scale / 2.0);
            PolygonPoint polygonPoint = new(num * Math.Cos(PI_2 * i / vertexCount), num * Math.Sin(PI_2 * i / vertexCount));
            array[i] = polygonPoint;
        }
        return new Polygon.Polygon(array);
    }

    public static Polygon.Polygon RandomCircleSweep2(double scale, int vertexCount)
    {
        double num = scale / 4.0;
        PolygonPoint[] array = new PolygonPoint[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            do
            {
                num += scale / 5.0 * (0.5 - RNG.NextDouble());
                num = (num > scale / 2.0) ? (scale / 2.0) : num;
                num = (num < scale / 10.0) ? (scale / 10.0) : num;
            }
            while (num < scale / 10.0 || num > scale / 2.0);
            PolygonPoint polygonPoint = new(num * Math.Cos(PI_2 * i / vertexCount), num * Math.Sin(PI_2 * i / vertexCount));
            array[i] = polygonPoint;
        }
        return new Polygon.Polygon(array);
    }
}
