using FarseerPhysics.Dynamics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public static class PlasticineConstants
    {
        public const float GrassOffset = 31f / 60f;

        public const float WideBorderOffset = -5f / 12f;

        public const int PlasticineBezierPoints = 3;

        public const float OutDiff = 1f / 15f;

        public const float TopOffset = 7f / 12f;

        public const float SurfaceOffset = 7f / 12f;

        public const float GroundOutOffset = 1f / 6f;

        public const float InnerDragOffset = -1f / 3f;

        public const float MaxOrtoOffset = 0.4f;

        public const float MaxDragOffset = 1.3333334f;

        public const float StartDragTime = 0.1f;

        public const float PlasticineDensity = 0.3f;

        public const float SurfaceWidth = 2.3333333f;

        public const float THICKESS = 5f / 6f;

        public const float PlasticinePartWidth = 0.6f;

        public const float PlasticineWidth = 0.6f;

        public static readonly LightColor BLUE = new(new Color(0f, 0f, 0f, 1f), ContreJourConstants.BlueLightColor.ChangeAlpha(1f));

        public static readonly LightColor Green = new(new Color(0f, 0f, 0f, 1f), ContreJourConstants.GreenLightColor.ChangeAlpha(1f));

        public static readonly LightColor LastLight = new(new Color(0f, 0f, 0f, 1f), new Color(2f / 51f, 2f / 51f, 2f / 51f, 1f));

        public static readonly LightColor WHITE = new(new Color(0f, 0f, 0f, 1f), ContreJourConstants.WhiteLightColor.ChangeAlpha(1f));

        public static readonly Color WhiteGroundColor = 13421772.ToRGBColor();

        public static readonly LightColor BlackLight = new(WhiteGroundColor, 10658466.ToRGBColor());

        public static readonly Color WhiteGroundOutColor = WhiteGroundColor.ChangeAlpha(0);

        public static readonly Color BlackBorderColor = ContreJourConstants.BlueLightColor.ChangeAlpha(byte.MaxValue);

        public static readonly Color BlackBorderOutColor = BlackBorderColor.ChangeAlpha(0);

        public static void ApplyStaticBodiesFilter(Body body)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                ApplyStaticBodiesFilter(fixture);
            }
        }

        public static void ApplyStaticBodiesFilter(Fixture fixture)
        {
            fixture.CollisionCategories = (Category)4;
        }

        public static void ApplyActiveBodiesFilter(Body body)
        {
            foreach (Fixture fixture in body.FixtureList)
            {
                ApplyActiveBodiesFilter(fixture);
            }
        }

        public static void ApplyActiveBodiesFilter(Fixture fixture)
        {
            fixture.CollisionCategories = (Category)2;
            fixture.CollidesWith = (Category)2147483641;
        }
    }
}
