using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D;

namespace Default.Namespace;

public class Portal : ParticleSystem
{
    private const int PARTS_COUNT = 5;

    protected List<Satellite> parts;

    protected float targetScale;

    protected float itemsScale;

    protected float scaleStep;

    public float TargetScale
    {
        get
        {
            return targetScale;
        }
        set
        {
            if (targetScale != value)
            {
                targetScale = value;
                if (targetScale != itemsScale)
                {
                    Visible = true;
                }
            }
        }
    }

    public float SpeedValue
    {
        get
        {
            return parts[0].SpeedValue;
        }
        set
        {
            for (int i = 0; i < 5; i++)
            {
                parts[i].SpeedValue = value;
            }
        }
    }

    public float ItemsScale
    {
        get
        {
            return itemsScale;
        }
        set
        {
            if (Maths.FuzzyNotEquals(itemsScale, value))
            {
                itemsScale = value;
                targetScale = value;
            }
        }
    }

    public float ScaleStep
    {
        get
        {
            return scaleStep;
        }
        set
        {
            scaleStep = value;
        }
    }

    public Portal(ContreJourGame game, Vector2 position)
        : this(game, position, "common/McFinishPart")
    {
    }

    public Portal(ContreJourGame game, Vector2 position, string textureName)
        : base(Mokus2DGame.LoadSpriteData(textureName))
    {
        parts = new List<Satellite>();
        base.Blend = BlendState.Additive;
        scaleStep = 0.05f;
        for (int i = 0; i < 5; i++)
        {
            Satellite item = new Satellite(game, AddParticle(), null, (float)Math.PI * 2f / 5f * (float)i, position);
            parts.Add(item);
        }
        SpeedValue = 40f;
        itemsScale = 1f;
        targetScale = 2f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (Maths.FuzzyNotEquals(itemsScale, targetScale))
        {
            itemsScale = Maths.StepTo(itemsScale, targetScale, scaleStep * time * 30f);
        }
        Visible = itemsScale > 0f;
    }

    public override void UpdateParticleTime(Particle particle, float time)
    {
        base.UpdateParticleTime(particle, time);
        particle.Scale = itemsScale;
    }
}
