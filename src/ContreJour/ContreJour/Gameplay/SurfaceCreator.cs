using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay;

public class SurfaceCreator
{
    public static PlasticineItem CreateParentPointsMaxWidth(ContreJourLevelBuilder builder, PlasticineBodyClip parent, List<Vector2> points, float maxWidth, out PlasticineItem leftItem)
    {
        if (builder.ContreJour.BlackSide)
        {
            maxWidth *= 1f;
        }
        List<Vector2> vertices = GetVertices(points, maxWidth);
        PlasticineItem plasticineItem = null;
        PlasticineItem plasticineItem2 = null;
        leftItem = null;
        int num = 0;
        bool flag = false;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            if (num <= 0 && Maths.Random() < 0.2f)
            {
                num = (int)Maths.Random(5f, 10f);
            }
            Vector2 vector = vertices[i];
            Vector2 vector2 = vertices[i + 1];
            float width = vector.DistanceTo(vector2);
            PlasticinePartBodyClip bodyClip = new(builder, PlasticineUtil.CreateSurfaceBodyWidthAnglePosition(angle: VectorUtil.Atan2(vector, vector2), position: GetPartCenterEnd(vector, vector2), world: builder.World, width: 0.6f), parent, width, !flag && (num > 0 || builder.ContreJour.WhiteSide || builder.ContreJour.RoseChapter || builder.ContreJour.BonusChapter));
            num--;
            PlasticineItem plasticineItem3 = new(bodyClip, width);
            if (leftItem == null || plasticineItem3.InitialPosition.X < leftItem.InitialPosition.X)
            {
                leftItem = plasticineItem3;
            }
            if (plasticineItem2 != null)
            {
                plasticineItem2.InsertAfter(plasticineItem3);
            }
            else
            {
                plasticineItem = plasticineItem3;
            }
            plasticineItem2 = plasticineItem3;
        }
        plasticineItem.InsertBefore(plasticineItem2);
        return plasticineItem;
    }

    public static Vector2 GetPartCenterEnd(Vector2 start, Vector2 end)
    {
        Vector2 point = end - start;
        Vector2 vector = VectorUtil.Center(start, end);
        VectorUtil.RotateMinus90(ref point);
        _ = VectorUtil.Normalize(ref point, 5f / 12f);
        return vector + point;
    }

    private static void GetBezierVerticesControlEndWidth(Vector2 start, Vector2 control, Vector2 end, float width, ref List<Vector2> result)
    {
        Bezier bezier = new(start, control, end);
        RopeMetrics ropeMetricsByLengthMaxPartSizeMinParts = RopeUtil.GetRopeMetricsByLengthMaxPartSizeMinParts(bezier.Length, width, 3);
        List<float> timesSequenceWithStepStartShift = bezier.GetTimesSequenceWithStepStartShift(ropeMetricsByLengthMaxPartSizeMinParts.PartSize, ropeMetricsByLengthMaxPartSizeMinParts.PartSize);
        for (int i = 0; i < ropeMetricsByLengthMaxPartSizeMinParts.Parts; i++)
        {
            Vector2 pointByTime = bezier.GetPointByTime(timesSequenceWithStepStartShift[i]);
            result.Add(pointByTime);
        }
    }

    public static List<Vector2> GetVertices(List<Vector2> points, float width)
    {
        List<Vector2> list = [];
        for (int i = 0; i < points.Count - 1; i++)
        {
            list.Add(VectorUtil.Center(points[i], points[i + 1]));
        }
        Vector2 item = VectorUtil.Center(points[^1], points[0]);
        list.Add(item);
        list.Insert(0, item);
        List<Vector2> result = [];
        result.Capacity = list.Count * 5;
        for (int j = 0; j < list.Count - 1; j++)
        {
            GetBezierVerticesControlEndWidth(list[j], points[j], list[j + 1], width, ref result);
        }
        result.Add(item);
        return result;
    }
}
