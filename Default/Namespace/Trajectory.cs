using System;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.OnOff;

namespace Default.Namespace;

public class Trajectory : ParticleSystem
{
    private readonly float enabledOpacity;

    public float Angle;

    public float FadeOutDelay;

    public float Impulse = 1f;

    private bool enabled;

    private readonly FadeAndHideEffect _fadeEffect;

    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled != value)
            {
                enabled = value;
                RefreshOpacity();
            }
        }
    }

    public Trajectory(ContreJourGame _game)
        : base(_game.BlackSide ? "chapter2/McTrampolinePathBlack" : "common/McTrampolinePath", 7)
    {
        enabledOpacity = (_game.Chapter == 4) ? 0.7f : 0.5f;
        Visible = false;
        OpacityFloat = 0f;
        int num = 0;
        _fadeEffect = new FadeAndHideEffect(this, 0.5f)
        {
            Clean = true
        };
        _fadeEffect.SetOn(value: false);
        foreach (Particle particle in Particles)
        {
            particle.OpacityFloat = 1 - (Math.Abs(num - 3) / 4);
            num++;
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        float num = Maths.Cos(Angle);
        float num2 = Maths.Sin(Angle);
        int num3 = 0;
        foreach (Particle particle in Particles)
        {
            particle.Position = new Vector2(num * (Impulse * num3 * 30f), (num2 * (Impulse * num3 * 30f)) - (2.2f * num3 * num3));
            num3++;
        }
    }

    private void RefreshOpacity()
    {
        _fadeEffect.IsOn = enabled;
    }
}
