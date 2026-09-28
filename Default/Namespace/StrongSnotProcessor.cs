using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class StrongSnotProcessor : SnotProcessor
{
    public StrongSnotProcessor(LevelBuilderBase _builder)
        : base(_builder, "strongSnot", 2f / 3f)
    {
    }

    public StrongSnotProcessor(LevelBuilderBase _builder, string _type, float _partSize)
        : base(_builder, _type, _partSize)
    {
    }

    public override float GetStartDensity()
    {
        return 10f;
    }

    public override float LinearDamping()
    {
        return 0f;
    }

    public override float GetDensityTotal(int index, int total)
    {
        return 0.13f + ((total - (float)index) / total * 0.13f);
    }

    public override Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 startPoint, Vector2 endPoint, int index, int total)
    {
        RevoluteJoint val = JointFactory.CreateRevoluteJoint(builder.World, startBody, endBody, endBody.Position - endPoint);
        val.CollideConnected = false;
        val.LimitEnabled = false;
        val.Broke += OnJointBroke;
        return (Joint)(object)val;
    }

    private void OnJointBroke(Joint arg1, float arg2)
    {
        throw new NotImplementedException();
    }
}
