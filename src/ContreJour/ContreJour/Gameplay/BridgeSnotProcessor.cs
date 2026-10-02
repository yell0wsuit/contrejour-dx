using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Mokus2D.Util.Data;

namespace ContreJour.Gameplay
{
    public class BridgeSnotProcessor : StrongSnotProcessor
    {
        public BridgeSnotProcessor(LevelBuilderBase builder)
            : base(builder, "bridgeSnot", 5f / 6f)
        {
        }

        public BridgeSnotProcessor(LevelBuilderBase builder, string type, float partSize)
            : base(builder, type, partSize)
        {
        }

        public override Body CreatePartBodyEndIndexTotalDensity(Vector2 startPoint, Vector2 endPoint, int index, int total, float density)
        {
            Vector2 position = endPoint;
            List<Vector2> list = [];
            GetLocalPartPositionsTotalStartEnd(index, total, ref startPoint, ref endPoint);
            Pair<Vector2> pointsPairStartEndWidthResult = ContreDrawUtil.GetPointsPairStartEndWidthResult(startPoint, startPoint, endPoint, 1f / 3f);
            Pair<Vector2> pointsPairStartEndWidthResult2 = ContreDrawUtil.GetPointsPairStartEndWidthResult(endPoint, startPoint, endPoint, 1f / 3f);
            list.Add(Builder.ToVec(pointsPairStartEndWidthResult.First));
            list.Add(Builder.ToVec(pointsPairStartEndWidthResult.Second));
            list.Add(Builder.ToVec(pointsPairStartEndWidthResult2.Second));
            list.Add(Builder.ToVec(pointsPairStartEndWidthResult2.First));
            Body val = FarseerUtil.CreateBox(Builder.World, position, list, sensor: false, density, dynamic: true);
            PlasticineConstants.ApplyActiveBodiesFilter(val);
            return val;
        }

        public virtual void GetLocalPartPositionsTotalStartEnd(int index, int total, ref Vector2 startPoint, ref Vector2 endPoint)
        {
            startPoint -= endPoint;
            endPoint = new Vector2(0f, 0f);
        }

        public override Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 startPoint, Vector2 endPoint, int index, int total)
        {
            return (Joint)(object)JointFactory.CreateRevoluteJoint(Builder.World, startBody, endBody, startPoint - endBody.Position);
        }
    }
}
