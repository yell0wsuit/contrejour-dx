using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;

namespace ContreJour.Gameplay
{
    public class EditorRevoluteJointProcessor(LevelBuilderBase builder) : RevoluteJointProcessor("editorRevoluteJoint", builder)
    {
        public override object ProcessItem(Hashtable item)
        {
            Hashtable hashtable = item.GetHashtable("config");
            Vector2 vector = item.GetVector("position");
            List<Body> bodiesByWorldReqResult = GetBodiesByWorldReqResult(vector, FarseerUtil.DynamicBodyPredicate);
            bodiesByWorldReqResult.AddRange(GetBodiesByWorldReqResult(vector, FarseerUtil.StaticBodyPredicate));
            if (hashtable.Exists("rotationLocked"))
            {
                hashtable["upperAngle"] = "0";
                hashtable["lowerAngle"] = "0";
            }
            if (bodiesByWorldReqResult.Count > 0)
            {
                if (bodiesByWorldReqResult.Count > 1)
                {
                    CreateRevoluteJointPositionConfig(bodiesByWorldReqResult, vector, hashtable);
                }
                else
                {
                    List<Body> bodiesByWorldReqResult2 = GetBodiesByWorldReqResult(vector, FarseerUtil.StaticBodyPredicate);
                    Body item2 = (bodiesByWorldReqResult2.Count > 0) ? bodiesByWorldReqResult2[0] : Builder.GroundBody;
                    List<Body> list = [item2, bodiesByWorldReqResult[0]];
                    CreateRevoluteJointPositionConfig(list, vector, hashtable);
                }
            }
            return null;
        }
    }
}
