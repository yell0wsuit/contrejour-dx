using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class JointProcessorBase(string _type, LevelBuilderBase _builder) : TypeProcessorBase(_type, _builder)
{
    public Joint CreateJointConfig(RevoluteJoint joint, Hashtable config)
    {
        builder.World.AddJoint((Joint)(object)joint);
        if (config.Exists("id"))
        {
            builder.CreatedObjects[config.GetString("id")] = joint;
        }
        return (Joint)(object)joint;
    }

    public Body GetBodyByWorld(Vector2 position)
    {
        return GetBodyByWorldReq(position, null);
    }

    public Body GetBodyByWorldReq(Vector2 position, Predicate<Body> match)
    {
        List<Body> bodiesByWorldReqResult = GetBodiesByWorldReqResult(position, match);
        Body val = TryGetBodyByType(bodiesByWorldReqResult, FarseerUtil.DynamicObjectPredicate);
        if (val != null)
        {
            return val;
        }
        Body val2 = TryGetBodyByType(bodiesByWorldReqResult, FarseerUtil.KinematicObjectPredicate);
        return val2 ?? (bodiesByWorldReqResult.Count > 0 ? bodiesByWorldReqResult[0] : builder.GroundBody);
    }

    private static Body TryGetBodyByType(List<Body> bodies, Predicate<object> type)
    {
        //IL_001d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0023: Expected O, but got Unknown
        List<object> list = MokusCollectionExtensions.Filter(bodies.ToArray(), type);
        return list.Count > 0 ? (Body)list[0] : null;
    }

    public List<Body> GetBodiesByWorldReqResult(Vector2 position, Predicate<Body> match)
    {
        List<Body> list = [];
        List<Body> list2 = [];
        foreach (Fixture item in builder.World.Query(position))
        {
            Body body = item.Body;
            if (!list2.Exists(body) && (match == null || match(body)))
            {
                list.Add(body);
                list2.Add(body);
            }
        }
        return list;
    }
}
