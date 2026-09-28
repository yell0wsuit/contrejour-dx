using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class EnergyPart : Satellite
{
    private float timeToEnd;

    private bool collected;

    private bool finished;

    private float opacity;

    protected override Vector2 TargetPosition => !collected ? base.TargetPosition : game.BonusTarget.BonusTarget();

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
        target = (BodyClip)game.BonusTarget;
        game.Hero.FinishEvent.AddListener(OnHeroFinish);
        collected = true;
        speedValue = Maths.Random(150f, 250f);
        angleStep = Maths.Random(0.05f, 0.1f);
    }

    public void OnHeroFinish()
    {
        game.Hero.FinishEvent.RemoveListener(OnHeroFinish);
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
        if (game == null)
        {
            return;
        }
        float num = clip.Position.DistanceTo(TargetPosition);
        if (collected && num < 50f)
        {
            clip.OpacityFloat = num / 50f;
        }
        else
        {
            clip.OpacityByte = 255;
        }
        if ((timeToEnd <= 0f && num <= 10f) || opacity <= 0f)
        {
            Remove();
            if (opacity > 0f && game.BonusTarget != null)
            {
                game.BonusTarget.ApplyBonus();
            }
        }
        else if (timeToEnd <= 0f)
        {
            angleStep += 0.005f;
            speedValue += 1f;
        }
        if (finished && opacity > 0f)
        {
            opacity = Maths.StepTo(opacity, 0f, 5f);
            clip.OpacityByte = (int)opacity;
        }
    }
}
