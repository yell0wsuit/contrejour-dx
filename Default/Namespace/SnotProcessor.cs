using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SnotProcessor(LevelBuilderBase builder, string type, float partSize) : JointProcessorBase(type, builder)
{
    public struct BodyAndJoint
    {
        public Body Body;

        public Joint Joint;
    }

    public const float EndBodyDensity = 0.221f;

    public const float DAMPING = 0.1f;

    public const float FREQUENCY = 5f;

    public const float DENSITY = 0.13f;

    public const float JointCircleRadius = 1f / 6f;

    public const float PartSize = 1.3333334f;

    public const float DefaultLinearDamping = 1f;

    protected float MaxPartSize { get; } = partSize;

    public SnotProcessor(LevelBuilderBase builder)
        : this(builder, "snot")
    {
    }

    public SnotProcessor(LevelBuilderBase builder, string type)
        : this(builder, type, 1.6f)
    {
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
        if (bodyClip is not null and (PlasticinePartBodyClip or EnergyBodyClip))
        {
            val = Builder.GroundBody;
        }
        if (bodyClip is ISnotHolder)
        {
            vector = ((ISnotHolder)bodyClip).SnotPosition;
        }
        Body val2 = Builder.World.CreateCircle(1f / 6f, vector, 0f, GetStartDensity(), dynamic: true);
        val2.SetSensor(value: true);
        PlasticineConstants.ApplyActiveBodiesFilter(val2);
        RevoluteJoint eyeJoint = FarseerUtil.CreateRevoluteJoint(Builder.World, val, val2, vector);
        Body previousBody = val2;
        List<Body> list = [];
        List<Joint> list2 = [];
        for (int i = 0; i < ropeMetricsEndItem.Parts; i++)
        {
            BodyAndJoint result = default;
            CreatePartStartEndResultIndexTotal(previousBody, ropeMetricsEndItem.GetPositionByIndex(i), ropeMetricsEndItem.GetPositionByIndex(i + 1), ref result, i, ropeMetricsEndItem.Parts);
            previousBody = result.Body;
            list.Add(result.Body);
            list2.Add(result.Joint);
        }
        return new SnotData(val2, eyeJoint, val, val.GetLocalPoint(vector), list, list2, ropeMetricsEndItem);
    }

    public virtual RopeMetricsWithCoords GetRopeMetricsEndItem(Vector2 startPoint, Vector2 endPoint, Hashtable item)
    {
        return RopeUtil.GetRopeMetricsEndMaxPartSizeMinParts(startPoint, endPoint, MaxPartSize, 3);
    }

    public virtual float LinearDamping()
    {
        return DefaultLinearDamping;
    }

    public virtual float GetDensityTotal(int index, int total)
    {
        return index != total - 1 ? 0.13f : 0.221f;
    }

    public void CreatePartStartEndResultIndexTotal(Body previousBody, Vector2 start, Vector2 end, ref BodyAndJoint result, int index, int total)
    {
        Body val = CreatePartBodyEndIndexTotalDensity(start, end, index, total, GetDensityTotal(index, total));
        val.LinearDamping = LinearDamping();
        result.Joint = JoinBodiesEndBodyStartEndIndexTotal(previousBody, val, start, end, index, total);
        result.Body = val;
    }

    public virtual Body CreatePartBodyEndIndexTotalDensity(Vector2 startPoint, Vector2 endPoint, int index, int total, float density)
    {
        Body val = Builder.World.CreateCircle(1f / 6f, endPoint, 0f, density, dynamic: true);
        val.SetSensor(value: true);
        return val;
    }

    public virtual Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 startPoint, Vector2 endPoint, int index, int total)
    {
        DistanceJoint val = JointFactory.CreateDistanceJoint(Builder.World, startBody, endBody, startPoint - startBody.Position, endPoint - endBody.Position, false);
        val.Frequency = 5f;
        val.DampingRatio = 0.1f;
        return (Joint)(object)val;
    }
}
