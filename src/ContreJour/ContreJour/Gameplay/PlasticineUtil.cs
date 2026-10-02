using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Mokus2D.Integration.Farseer.Util;

namespace ContreJour.Gameplay
{
    public class PlasticineUtil
    {
        public static readonly object LIMIT = new();

        public static PolygonShape CreateSurfaceBox(float width)
        {
            //IL_0005: Unknown result type (might be due to invalid IL or missing references)
            //IL_000b: Expected O, but got Unknown
            PolygonShape val = new(0.3f);
            val.SetAsBox(width / 2f, 5f / 12f);
            return val;
        }

        public static Body CreateSurfaceBodyWidthAnglePosition(World world, float width, float angle, Vector2 position)
        {
            Body val = FarseerUtil.CreateBox(world, position, width, 5f / 6f, angle, sensor: false, 0.3f, dynamic: false);
            val.BodyType = (BodyType)1;
            float y = -0.625f;
            Fixture val2 = FixtureFactory.AttachEdge(new Vector2((0f - width) / 2f, y), new Vector2(width / 2f, y), val, 0.3f);
            PlasticineConstants.ApplyStaticBodiesFilter(val2);
            val2.UserData = LIMIT;
            PlasticineConstants.ApplyStaticBodiesFilter(val);
            val.LinearDamping = 10f;
            val.AngularDamping = 10f;
            val.Inertia *= 10f;
            return val;
        }

        public static PlasticineItem GetItemCountDirection(PlasticineItem start, int count, int direction)
        {
            PlasticineItem plasticineItem = start;
            for (int i = 0; i < count; i++)
            {
                plasticineItem = (direction >= 0) ? plasticineItem.NextItem : plasticineItem.PreviousItem;
            }
            return plasticineItem;
        }
    }
}
