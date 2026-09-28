using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class TrampolineSnotProcessor(LevelBuilderBase builder) : BridgeSnotProcessor(builder, "trampoline", 2f / 3f)
{
    private List<TrampolinePartBodyClip> parts = [];

    private static readonly float StartRadius = 5f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private readonly float PartAngle = MathHelper.ToRadians(15f);

    public override float GetDensityTotal(int index, int total)
    {
        return 2f;
    }

    public override void GetLocalPartPositionsTotalStartEnd(int index, int total, ref Vector2 startPoint, ref Vector2 endPoint)
    {
        startPoint -= endPoint;
        endPoint = startPoint;
        if (index != 0)
        {
            startPoint *= 1.25f;
        }
        if (index != total - 1)
        {
            endPoint *= -0.25f;
        }
        else
        {
            endPoint = new Vector2(0f, 0f);
        }
    }

    public override RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 startPoint, Vector2 endPoint, Hashtable item)
    {
        RopeMetricsWithCoords ropeMetricsWithCoords = base.GetRopeMetricsEndItem(startPoint, endPoint, item);
        if (ropeMetricsWithCoords.Parts % 2 == 0)
        {
            ropeMetricsWithCoords = RopeUtil.GetRopeMetricsEndMaxPartSizeMinParts(startPoint, endPoint, partSize, ropeMetricsWithCoords.Parts + 1);
        }
        return ropeMetricsWithCoords;
    }

    public override Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 startPoint, Vector2 endPoint, int index, int total)
    {
        Vector2 vector = startPoint;
        vector -= endPoint;
        Vector2 vector2 = vector;
        vector += endPoint;
        vector2 += endPoint;
        DistanceJoint val = JointFactory.CreateDistanceJoint(builder.World, startBody, endBody, vector2 - startBody.Position, vector - endBody.Position, false);
        val.Length = 0f;
        val.Frequency = 4.5f;
        val.DampingRatio = 1f;
        return (Joint)(object)val;
    }

    public override Body CreatePartBodyEndIndexTotalDensity(Vector2 startPoint, Vector2 endPoint, int index, int total, float density)
    {
        Body val = base.CreatePartBodyEndIndexTotalDensity(startPoint, endPoint, index, total, density);
        PlasticineConstants.ApplyActiveBodiesFilter(val);
        TrampolinePartBodyClip item = new(builder, val);
        parts.Add(item);
        val.AngularDamping = 20f;
        Body val2 = null;
        if (index == 0)
        {
            val2 = builder.World.CreateCircle(StartRadius, startPoint + new Vector2(0f, StartRadius));
        }
        else if (index == total - 1)
        {
            val2 = builder.World.CreateCircle(StartRadius, endPoint + new Vector2(0f, StartRadius));
        }
        if (val2 != null)
        {
            PlasticineConstants.ApplyActiveBodiesFilter(val2);
        }
        return val;
    }

    public override object ProcessItem(Hashtable item)
    {
        parts.Clear();
        SnotData snotData = (SnotData)base.ProcessItem(item);
        foreach (TrampolinePartBodyClip part in parts)
        {
            part.Data = snotData;
        }
        Vector2 vector = item.GetVector("end");
        Vector2 vector2 = item.GetVector("start");
        _ = FarseerUtil.CreateRevoluteJoint(builder.World, builder.GroundBody, snotData.EndBody, vector);
        _ = FarseerUtil.CreateRevoluteJoint(builder.World, builder.GroundBody, snotData.FirstBody, vector2);
        builder.World.RemoveBody(snotData.EyeBody);
        snotData.EyeBody = null;
        return snotData;
    }
}
