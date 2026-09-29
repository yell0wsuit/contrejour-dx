using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class StickyBodyClip : ContreJourBodyClip
    {
        private Body joinedBody;

        private bool joined;

        private Vector2 offset;

        public StickyBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            Body.BodyType = (BodyType)1;
        }

        private void Join()
        {
            //IL_004c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0059: Unknown result type (might be due to invalid IL or missing references)
            //IL_005f: Expected O, but got Unknown
            joinedBody = Builder.GroundBody;
            Builder.GameRoot.ChangeChildLayer(Clip, -1);
            CircleShape val = null;
            foreach (Fixture fixture in Body.FixtureList)
            {
                if (fixture.Shape.ShapeType == 0)
                {
                    val = (CircleShape)fixture.Shape;
                    break;
                }
            }
            Vector2 worldPoint = Body.GetWorldPoint(val.Position);
            List<Fixture> list = Builder.World.Query(worldPoint, 1.6666666f, 1.6666666f);
            float? num = null;
            foreach (Fixture item in list)
            {
                float num2 = Vector2.Distance(item.Body.Position, worldPoint);
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
}
