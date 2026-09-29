using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public class EditorRevoluteJointProcessor(LevelBuilderBase builder) : RevoluteJointProcessor("editorRevoluteJoint", builder)
    {
        public override object ProcessItem(Hashtable item)
        {
            Hashtable hashtable = item.GetHashtable("config");
            Vector2 vector = item.GetVector("position");
            List<Body> bodiesByWorldReqResult = GetBodiesByWorldReqResult(vector, FarseerUtil.DynamicBodyPredicate);
            bodiesByWorldReqResult.AddItemsNoGarbage(GetBodiesByWorldReqResult(vector, FarseerUtil.StaticBodyPredicate));
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
                    Body item2 = (bodiesByWorldReqResult2.Count > 0) ? bodiesByWorldReqResult2.First() : Builder.GroundBody;
                    List<Body> list = [item2, bodiesByWorldReqResult.First()];
                    CreateRevoluteJointPositionConfig(list, vector, hashtable);
                }
            }
            return null;
        }
    }
}
