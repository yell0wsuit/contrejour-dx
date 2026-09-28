using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SnotProcessor : JointProcessorBase
{
    public struct BodyAndJoint
    {
        public Body Body;

        public Joint Joint;
    }

    public const float END_BODY_DENSITY = 0.221f;

    public const float DAMPING = 0.1f;

    public const float FREQUENCY = 5f;

    public const float DENSITY = 0.13f;

    public const float JOINT_CIRCLE_RADIUS = 1f / 6f;

    public const float PART_SIZE = 1.3333334f;

    public const float LINEAR_DAMPING = 1f;

    protected float partSize;

    public SnotProcessor(LevelBuilderBase _builder)
        : this(_builder, "snot")
    {
    }

    public SnotProcessor(LevelBuilderBase _builder, string _type)
        : this(_builder, _type, 1.6f)
    {
    }

    public SnotProcessor(LevelBuilderBase _builder, string _type, float _partSize)
        : base(_type, _builder)
    {
        partSize = _partSize;
    }

    public virtual float GetStartDensity()
    {
        return 0.13f;
    }

    public override object ProcessItem(Hashtable item)
    {
        Vector2 vector = item.GetVector("start");
        RopeMetricsWithCoords ropeMetricsEndItem = GetRopeMetricsEndItem(vector, item.GetVector("end"), item);
        Body val = GetBodyByWorld(vector);
        BodyClip bodyClip = val.UserData as BodyClip;
        if (bodyClip != null && (bodyClip is PlasticinePartBodyClip || bodyClip is EnergyBodyClip))
        {
            val = builder.GroundBody;
        }
        if (bodyClip is ISnotHolder)
        {
            vector = ((ISnotHolder)bodyClip).SnotPosition;
        }
        Body val2 = builder.World.CreateCircle(1f / 6f, vector, 0f, GetStartDensity(), dynamic: true);
        val2.SetSensor(value: true);
        PlasticineConstants.ApplyActiveBodiesFilter(val2);
        RevoluteJoint eyeJoint = FarseerUtil.CreateRevoluteJoint(builder.World, val, val2, vector);
        Body previousBody = val2;
        List<Body> list = new List<Body>();
        List<Joint> list2 = new List<Joint>();
        for (int i = 0; i < ropeMetricsEndItem.Parts; i++)
        {
            BodyAndJoint result = default(BodyAndJoint);
            CreatePartStartEndResultIndexTotal(previousBody, ropeMetricsEndItem.GetPositionByIndex(i), ropeMetricsEndItem.GetPositionByIndex(i + 1), ref result, i, ropeMetricsEndItem.Parts);
            previousBody = result.Body;
            list.Add(result.Body);
            list2.Add(result.Joint);
        }
        return new SnotData(val2, eyeJoint, val, val.GetLocalPoint(vector), list, list2, ropeMetricsEndItem);
    }

    public virtual RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 start, Vector2 end, Hashtable item)
    {
        return RopeUtil.GetRopeMetricsEndMaxPartSizeMinParts(start, end, partSize, 3);
    }

    public virtual float LinearDamping()
    {
        return 1f;
    }

    public virtual float GetDensityTotal(int index, int total)
    {
        if (index != total - 1)
        {
            return 0.13f;
        }
        return 0.221f;
    }

    public void CreatePartStartEndResultIndexTotal(Body previousBody, Vector2 start, Vector2 end, ref BodyAndJoint result, int index, int total)
    {
        Body val = CreatePartBodyEndIndexTotalDensity(start, end, index, total, GetDensityTotal(index, total));
        val.LinearDamping = LinearDamping();
        result.Joint = JoinBodiesEndBodyStartEndIndexTotal(previousBody, val, start, end, index, total);
        result.Body = val;
    }

    public virtual Body CreatePartBodyEndIndexTotalDensity(Vector2 start, Vector2 end, int index, int total, float density)
    {
        Body val = builder.World.CreateCircle(1f / 6f, end, 0f, density, dynamic: true);
        val.SetSensor(value: true);
        return val;
    }

    public virtual Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 start, Vector2 end, int index, int total)
    {
        DistanceJoint val = JointFactory.CreateDistanceJoint(builder.World, startBody, endBody, start - startBody.Position, end - endBody.Position, false);
        val.Frequency = 5f;
        val.DampingRatio = 0.1f;
        return (Joint)(object)val;
    }
}
