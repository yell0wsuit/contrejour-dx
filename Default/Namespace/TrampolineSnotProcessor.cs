using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class TrampolineSnotProcessor(LevelBuilderBase _builder) : BridgeSnotProcessor(_builder, "trampoline", 2f / 3f)
{
    private const float SPRING_K = 800f;

    protected List<TrampolinePartBodyClip> parts = new List<TrampolinePartBodyClip>();

    private static readonly float START_RADIUS = 5f * Box2DConfig.DefaultConfig.SizeMultiplier;

    public readonly float PART_ANGLE = MathHelper.ToRadians(15f);

    public override float GetDensityTotal(int index, int total)
    {
        return 2f;
    }

    public override void GetLocalPartPositionsTotalStartEnd(int index, int total, ref Vector2 start, ref Vector2 end)
    {
        start -= end;
        end = start;
        if (index != 0)
        {
            start *= 1.25f;
        }
        if (index != total - 1)
        {
            end *= -0.25f;
        }
        else
        {
            end = new Vector2(0f, 0f);
        }
    }

    public override RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 start, Vector2 end, Hashtable item)
    {
        RopeMetricsWithCoords ropeMetricsWithCoords = base.GetRopeMetricsEndItem(start, end, item);
        if (ropeMetricsWithCoords.Parts % 2 == 0)
        {
            ropeMetricsWithCoords = RopeUtil.GetRopeMetricsEndMaxPartSizeMinParts(start, end, partSize, ropeMetricsWithCoords.Parts + 1);
        }
        return ropeMetricsWithCoords;
    }

    public override Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 start, Vector2 end, int index, int total)
    {
        Vector2 vector = start;
        vector -= end;
        Vector2 vector2 = vector;
        vector += end;
        vector2 += end;
        DistanceJoint val = JointFactory.CreateDistanceJoint(builder.World, startBody, endBody, vector2 - startBody.Position, vector - endBody.Position, false);
        val.Length = 0f;
        val.Frequency = 4.5f;
        val.DampingRatio = 1f;
        return (Joint)(object)val;
    }

    public override Body CreatePartBodyEndIndexTotalDensity(Vector2 start, Vector2 end, int index, int total, float density)
    {
        Body val = base.CreatePartBodyEndIndexTotalDensity(start, end, index, total, density);
        PlasticineConstants.ApplyActiveBodiesFilter(val);
        TrampolinePartBodyClip item = new TrampolinePartBodyClip(builder, val);
        parts.Add(item);
        val.AngularDamping = 20f;
        Body val2 = null;
        if (index == 0)
        {
            val2 = builder.World.CreateCircle(START_RADIUS, start + new Vector2(0f, START_RADIUS));
        }
        else if (index == total - 1)
        {
            val2 = builder.World.CreateCircle(START_RADIUS, end + new Vector2(0f, START_RADIUS));
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
        FarseerUtil.CreateRevoluteJoint(builder.World, builder.GroundBody, snotData.EndBody, vector);
        FarseerUtil.CreateRevoluteJoint(builder.World, builder.GroundBody, snotData.FirstBody, vector2);
        builder.World.RemoveBody(snotData.EyeBody);
        snotData.EyeBody = null;
        return snotData;
    }
}
