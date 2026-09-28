using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class EditorRevoluteJointProcessor : RevoluteJointProcessor
{
    public EditorRevoluteJointProcessor(LevelBuilderBase _builder)
        : base("editorRevoluteJoint", _builder)
    {
    }

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
                Body item2 = (bodiesByWorldReqResult2.Count > 0) ? bodiesByWorldReqResult2.First() : builder.GroundBody;
                List<Body> list = [item2, bodiesByWorldReqResult.First()];
                CreateRevoluteJointPositionConfig(list, vector, hashtable);
            }
        }
        return null;
    }
}
