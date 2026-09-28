using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Data;

namespace Default.Namespace;

public class BridgeSnotProcessor : StrongSnotProcessor
{
    private const float BRIDGE_WIDTH = 1f / 3f;

    private const float BRIDGE_PART_SIZE = 5f / 6f;

    public BridgeSnotProcessor(LevelBuilderBase _builder)
        : base(_builder, "bridgeSnot", 5f / 6f)
    {
    }

    public BridgeSnotProcessor(LevelBuilderBase _builder, string _type, float _partSize)
        : base(_builder, _type, _partSize)
    {
    }

    public override Body CreatePartBodyEndIndexTotalDensity(Vector2 start, Vector2 end, int index, int total, float density)
    {
        Vector2 position = end;
        List<Vector2> list = new List<Vector2>();
        GetLocalPartPositionsTotalStartEnd(index, total, ref start, ref end);
        Pair<Vector2> pointsPairStartEndWidthResult = ContreDrawUtil.GetPointsPairStartEndWidthResult(start, start, end, 1f / 3f);
        Pair<Vector2> pointsPairStartEndWidthResult2 = ContreDrawUtil.GetPointsPairStartEndWidthResult(end, start, end, 1f / 3f);
        list.Add(builder.ToVec(pointsPairStartEndWidthResult.First));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult.Second));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult2.Second));
        list.Add(builder.ToVec(pointsPairStartEndWidthResult2.First));
        Body val = FarseerUtil.CreateBox(builder.World, position, list, sensor: false, density, dynamic: true);
        PlasticineConstants.ApplyActiveBodiesFilter(val);
        return val;
    }

    public virtual void GetLocalPartPositionsTotalStartEnd(int index, int total, ref Vector2 start, ref Vector2 end)
    {
        start -= end;
        end = new Vector2(0f, 0f);
    }

    public override Joint JoinBodiesEndBodyStartEndIndexTotal(Body startBody, Body endBody, Vector2 start, Vector2 end, int index, int total)
    {
        return (Joint)(object)JointFactory.CreateRevoluteJoint(builder.World, startBody, endBody, start - endBody.Position);
    }
}
