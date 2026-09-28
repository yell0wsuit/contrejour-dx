using System.Collections.Generic;

using ContreJour.Debug;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class RevoluteJointProcessor : JointProcessorBase
{
    public RevoluteJointProcessor(LevelBuilderBase builder)
        : base("revoluteJoint", builder)
    {
    }

    public RevoluteJointProcessor(string type, LevelBuilderBase builder)
        : base(type, builder)
    {
    }

    public void CreateRevoluteJointPositionConfig(List<Body> bodies, Vector2 position, Hashtable config)
    {
        //IL_0024: Unknown result type (might be due to invalid IL or missing references)
        //IL_002a: Invalid comparison between Unknown and I4
        //IL_0068: Unknown result type (might be due to invalid IL or missing references)
        //IL_006e: Expected O, but got Unknown
        //IL_0033: Unknown result type (might be due to invalid IL or missing references)
        if (bodies[0] == null || bodies[1] == null)
        {
            DebugUtil.Trace("Revolute joint not initialized");
            return;
        }
        if ((int)bodies[0].BodyType == 2 && bodies[1].BodyType == 0)
        {
            (bodies[1], bodies[0]) = (bodies[0], bodies[1]);
        }
        RevoluteJoint val = new(bodies[0], bodies[1], position, false);
        if (config.Exists("motorSpeed"))
        {
            float num = 0f;
            for (int i = 0; i < bodies.Count; i++)
            {
                num += bodies[i].Inertia;
            }
            val.MotorSpeed = config.GetFloat("motorSpeed");
            val.MaxMotorTorque = config.Exists("maxMotorTorque") ? config.GetFloat("maxMotorTorque") : (30f * num);
            val.MotorEnabled = true;
        }
        if (config.Exists("upperAngle"))
        {
            val.LimitEnabled = true;
            val.LowerLimit = MathHelper.ToRadians(config.GetFloat("lowerAngle"));
            val.UpperLimit = MathHelper.ToRadians(config.GetFloat("upperAngle"));
        }
        val.CollideConnected = false;
        _ = CreateJointConfig(val, config);
    }
}
