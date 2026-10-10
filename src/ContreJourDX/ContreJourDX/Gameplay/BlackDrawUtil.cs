using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace ContreJourDX.Gameplay
{
    public class BlackDrawUtil
    {
        public static void Resize(List<List<Vector2>> polygons, int size, int pointsInPolygon)
        {
            polygons.Resize(size);
            for (int i = 0; i < size; i++)
            {
                polygons[i].Resize(pointsInPolygon);
            }
        }

        public static int CreatePolygons(List<List<Vector2>> polygons, List<Pair<Vector2>> pairs, float minDistance)
        {
            int num = 0;
            Pair<Vector2> pair = pairs[0];
            for (int i = 0; i < pairs.Count - 1; i++)
            {
                if ((pair.First - pairs[i + 1].First).Length() > minDistance)
                {
                    List<Vector2> polygon = polygons[num];
                    num++;
                    CreatePolygon(polygon, pair, pairs[i + 1]);
                    pair = pairs[i + 1];
                }
            }
            return num;
        }

        public static void CreatePolygons(List<List<Vector2>> polygons, List<Pair<Vector2>> pairs)
        {
            for (int i = 0; i < pairs.Count - 1; i++)
            {
                CreatePolygon(polygons[i], pairs[i], pairs[i + 1]);
            }
        }

        public static void CreatePolygon(List<Vector2> polygon, Pair<Vector2> pair, Pair<Vector2> nextPair)
        {
            polygon[0] = pair.First;
            polygon[1] = nextPair.First;
            polygon[2] = nextPair.Second;
            polygon[3] = pair.Second;
        }

        public static void CreatePolygons(List<List<Vector2>> polygons, List<Vector2> firstBezier, List<Vector2> secondBezier)
        {
            for (int i = 0; i < firstBezier.Count - 1; i++)
            {
                List<Vector2> list = polygons[i];
                list[0] = firstBezier[i];
                list[1] = firstBezier[i + 1];
                list[2] = secondBezier[i + 1];
                list[3] = secondBezier[i];
            }
        }

        public static void SetBorderColors(int allPointsSize, Color startColor, Color endColor, Color outColor, Vertex[] resultColors)
        {
            GraphUtil.CreateGradientColors(0, allPointsSize / 2, startColor, endColor, outColor, resultColors);
            GraphUtil.CreateGradientColors(allPointsSize / 2, allPointsSize, endColor, startColor, outColor, resultColors);
        }
    }
}
