using System;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class WhiteGrassController : GrassController
{
    private const float BORDER_OFFSET = -7f;

    private const int WHITE_GRASS_COUNT = 3;

    protected bool borderUpdated;

    public override float SmallGrassScale => 0.7f;

    public override int GrassFrame => Maths.Random(3);

    public override int SmallGrassFrame => GrassFrame;

    public override float WindAngle => (float)Math.PI / 8f;

    public override float TrampleAngle => (float)Math.PI / 12f;

    public override float SmallGrassStep => base.SmallGrassStep / 2f;

    public override float GrassStep => base.GrassStep / 2f;

    public float SmallGrassOffset(int index)
    {
        return ((index * 2) - 1) * plasticine.Width / 3f;
    }

    public WhiteGrassController(PlasticinePartBodyClip _plasticine)
        : base(_plasticine)
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!borderUpdated)
        {
            if (plasticine.Item.PreviousItem.BodyClip.GrassController == null)
            {
                GrassAndPosition grassAndPosition = smallGrasses[0];
                grassAndPosition.Position = new Vector2(grassAndPosition.Position.X, -7f * builder.EngineConfig.SizeMultiplier);
                grassAndPosition.Particle.Scale = 0.4f;
            }
            if (plasticine.Item.NextItem.BodyClip.GrassController == null)
            {
                GrassAndPosition grassAndPosition2 = smallGrasses[1];
                grassAndPosition2.Position = new Vector2(grassAndPosition2.Position.X, -7f * builder.EngineConfig.SizeMultiplier);
                grassAndPosition2.Particle.Scale = 0.4f;
            }
            borderUpdated = true;
        }
    }

    public override float GetSmallGrassOffset(int index)
    {
        return index != 0 ? plasticine.Width / 2f : (0f - plasticine.Width) / 2f;
    }
}
