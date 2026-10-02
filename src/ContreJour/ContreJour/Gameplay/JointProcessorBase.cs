using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace ContreJour.Gameplay
{
    public class JointProcessorBase(string type, LevelBuilderBase builder) : TypeProcessorBase(type, builder)
    {
        public Joint CreateJointConfig(RevoluteJoint joint, Hashtable config)
        {
            Builder.World.AddJoint((Joint)(object)joint);
            if (config.Exists("id"))
            {
                Builder.CreatedObjects[config.GetString("id")] = joint;
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
            return val2 ?? (bodiesByWorldReqResult.Count > 0 ? bodiesByWorldReqResult[0] : Builder.GroundBody);
        }

        private static Body TryGetBodyByType(List<Body> bodies, Predicate<object> type)
        {
            return bodies.Find(type);
        }

        public List<Body> GetBodiesByWorldReqResult(Vector2 position, Predicate<Body> match)
        {
            List<Body> list = [];
            List<Body> list2 = [];
            foreach (Fixture item in Builder.World.Query(position))
            {
                Body body = item.Body;
                if (!list2.Contains(body) && (match == null || match(body)))
                {
                    list.Add(body);
                    list2.Add(body);
                }
            }
            return list;
        }
    }
}
