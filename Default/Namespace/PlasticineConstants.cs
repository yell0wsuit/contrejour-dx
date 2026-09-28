using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public static class PlasticineConstants
{
    public const float GRASS_OFFSET = 31f / 60f;

    public const float WIDE_BORDER_OFFSET = -5f / 12f;

    public const int PLASTICINE_BEZIER_POINTS = 3;

    public const float OUT_DIFF = 1f / 15f;

    public const float TOP_OFFSET = 7f / 12f;

    public const float SURFACE_OFFSET = 7f / 12f;

    public const float GROUND_OUT_OFFSET = 1f / 6f;

    public const float INNER_DRAG_OFFSET = -1f / 3f;

    public const float MAX_ORTO_OFFSET = 0.4f;

    public const float MAX_DRAG_OFFSET = 1.3333334f;

    public const float START_DRAG_TIME = 0.1f;

    public const float PLASTICINE_DENSITY = 0.3f;

    public const float SURFACE_WIDTH = 2.3333333f;

    public const float THICKESS = 5f / 6f;

    public const float PLASTICINE_PART_WIDTH = 0.6f;

    public const float PLASTICINE_WIDTH = 0.6f;

    public static readonly LightColor BLUE = new LightColor(new Color(0f, 0f, 0f, 1f), ContreJourConstants.BLUE_LIGHT_COLOR.ChangeAlpha(1f));

    public static readonly LightColor Green = new LightColor(new Color(0f, 0f, 0f, 1f), ContreJourConstants.GreenLightColor.ChangeAlpha(1f));

    public static readonly LightColor LAST_LIGHT = new LightColor(new Color(0f, 0f, 0f, 1f), new Color(2f / 51f, 2f / 51f, 2f / 51f, 1f));

    public static readonly LightColor WHITE = new LightColor(new Color(0f, 0f, 0f, 1f), ContreJourConstants.WHITE_LIGHT_COLOR.ChangeAlpha(1f));

    public static readonly Color WHITE_GROUND_COLOR = 13421772.ToRGBColor();

    public static readonly LightColor BLACK_LIGHT = new LightColor(WHITE_GROUND_COLOR, 10658466.ToRGBColor());

    public static readonly Color WHITE_GROUND_OUT_COLOR = WHITE_GROUND_COLOR.ChangeAlpha(0);

    public static readonly Color BLACK_BORDER_COLOR = ContreJourConstants.BLUE_LIGHT_COLOR.ChangeAlpha(byte.MaxValue);

    public static readonly Color BLACK_BORDER_OUT_COLOR = BLACK_BORDER_COLOR.ChangeAlpha(0);

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
