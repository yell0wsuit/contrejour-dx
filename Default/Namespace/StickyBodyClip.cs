using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class StickyBodyClip : ContreJourBodyClip
{
    private const float PLAY_SPEED = 3f;

    private const float RADIUS = 1.6666666f;

    protected Body joinedBody;

    protected bool joined;

    private Vector2 offset;

    public StickyBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        Body.BodyType = (BodyType)1;
    }

    private void Join()
    {
        //IL_004c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0059: Unknown result type (might be due to invalid IL or missing references)
        //IL_005f: Expected O, but got Unknown
        joinedBody = builder.GroundBody;
        builder.GameRoot.ChangeChildLayer(clip, -1);
        CircleShape val = null;
        foreach (Fixture fixture in Body.FixtureList)
        {
            if ((int)fixture.Shape.ShapeType == 0)
            {
                val = (CircleShape)fixture.Shape;
                break;
            }
        }
        Vector2 worldPoint = Body.GetWorldPoint(val.Position);
        List<Fixture> list = builder.World.Query(worldPoint, 1.6666666f, 1.6666666f);
        float? num = null;
        foreach (Fixture item in list)
        {
            float num2 = item.Body.Position.DistanceTo(worldPoint);
            if (item.Body != Body && (!num.HasValue || num2 < num))
            {
                joinedBody = item.Body;
                num = num2;
            }
        }
        offset = Body.Position - joinedBody.Position;
        Body.SleepingAllowed = false;
    }

    public override void Update(float time)
    {
        if (time != 0f && !joined)
        {
            joined = true;
            Join();
        }
        if (joined)
        {
            Body.Position = joinedBody.Position + offset;
        }
        base.Update(time);
    }
}
