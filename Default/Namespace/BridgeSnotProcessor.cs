using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;

namespace Default.Namespace;

public class BridgeSnotProcessor : StrongSnotProcessor
{
    public BridgeSnotProcessor(LevelBuilderBase _builder)
        : base(_builder, "bridgeSnot", 5f / 6f)
    {
    }

    public BridgeSnotProcessor(LevelBuilderBase _builder, string _type, float _partSize)
        : base(_builder, _type, _partSize)
    {
    }

    public override Body CreatePartBodyEndIndexTotalDensity(Vector2 startPoint, Vector2 endPoint, int index, int total, float density)
    {
        Vector2 position = endPoint;
        List<Vector2> list = [];
        GetLocalPartPositionsTotalStartEnd(index, total, ref startPoint, ref endPoint);
        Pair<Vector2> pointsPairStartEndWidthResult = ContreDrawUtil.GetPointsPairStartEndWidthResult(startPoint, startPoint, endPoint, 1f / 3f);
        Pair<Vector2> pointsPairStartEndWidthResult2 = ContreDrawUtil.GetPointsPairStartEndWidthResult(endPoint, startPoint, endPoint, 1f / 3f);
        list.Add(builder.ToVec(pointsPairStartEndWidthResult.First));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult.Second));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult2.Second));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult2.First));
        Body val = FarseerUtil.CreateBox(builder.World, position, list, sensor: false, density, dynamic: true);
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
        return (Joint)(object)JointFactory.CreateRevoluteJoint(builder.World, startBody, endBody, startPoint - endBody.Position);
    }
}
