using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay;

public class EnergyPart : Satellite
{
    private float timeToEnd;

    private bool collected;

    private bool finished;

    private float opacity;

    protected override Vector2 TargetPosition => !collected ? base.TargetPosition : Game.BonusTarget.BonusTarget();

    public EnergyPart(ContreJourGame game, BodyClip parent, float direction, Vector2 position)
        : this(game, parent, game.Energy.AddOrGetInvisible(), direction, position)
    {
    }

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
    public EnergyPart(ContreJourGame game, BodyClip parent, Particle particle, float direction, Vector2 position)
        : base(game, particle, parent, direction, position)
    {
        timeToEnd = Maths.Random(1f, 2f);
        collected = false;
        opacity = 255f;
    }

    public void Collect()
    {
        Target = (BodyClip)Game.BonusTarget;
        Game.Hero.FinishEvent.AddListener(OnHeroFinish);
        collected = true;
        SpeedValue = Maths.Random(150f, 250f);
        AngleStep = Maths.Random(0.05f, 0.1f);
    }

    public void OnHeroFinish()
    {
        Game.Hero.FinishEvent.RemoveListener(OnHeroFinish);
        collected = false;
        finished = true;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (collected)
        {
            timeToEnd -= time;
        }
        if (Game == null)
        {
            return;
        }
        float num = Clip.Position.DistanceTo(TargetPosition);
        if (collected && num < 50f)
        {
            Clip.OpacityFloat = num / 50f;
        }
        else
        {
            Clip.OpacityByte = 255;
        }
        if ((timeToEnd <= 0f && num <= 10f) || opacity <= 0f)
        {
            Remove();
            if (opacity > 0f && Game.BonusTarget != null)
            {
                Game.BonusTarget.ApplyBonus();
            }
        }
        else if (timeToEnd <= 0f)
        {
            AngleStep += 0.005f;
            SpeedValue += 1f;
        }
        if (finished && opacity > 0f)
        {
            opacity = Maths.StepTo(opacity, 0f, 5f);
            Clip.OpacityByte = (int)opacity;
        }
    }
}
