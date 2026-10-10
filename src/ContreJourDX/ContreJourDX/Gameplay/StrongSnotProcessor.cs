using System;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace ContreJourDX.Gameplay
{
    public class StrongSnotProcessor : SnotProcessor
    {
        public StrongSnotProcessor(LevelBuilderBase builder)
            : base(builder, "strongSnot", 2f / 3f)
        {
        }

        public StrongSnotProcessor(LevelBuilderBase builder, string type, float partSize)
            : base(builder, type, partSize)
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
            RevoluteJoint val = JointFactory.CreateRevoluteJoint(Builder.World, startBody, endBody, endBody.Position - endPoint);
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
}
