using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class Satellite : IUpdatable, IRemovable
{
    private float direction;

    protected float speedValue;

    protected float angleStep;

    private bool hasRemove;

    protected Particle clip;

    protected ContreJourGame game;

    private Vector2 initialPosition;

    protected BodyClip target;

    public Particle Clip => clip;

    public float SpeedValue
    {
        get => speedValue;
        set => speedValue = value;
    }

    public float AngleStep
    {
        get => angleStep;
        set => angleStep = value;
    }

    protected virtual Vector2 TargetPosition => target == null ? initialPosition : game.Builder.ToIPadPoint(target.Body.Position);

    public bool ShouldRemove => hasRemove;

    public Satellite(ContreJourGame game, Particle clip, BodyClip parent, float direction, Vector2 position)
    {
        target = parent;
        initialPosition = position;
        this.clip = clip;
        speedValue = Maths.Random(15f, 25f) * 2f;
        angleStep = Maths.Random(0.18f, 0.28f);
        this.direction = direction;
        this.clip.Position = position;
        if (game != null)
        {
            this.game = game;
            this.game.AddUpdatable(this);
        }
        hasRemove = false;
    }

    public virtual void Update(float time)
    {
        float num = Math.Min(time, 1f / 30f);
        Vector2 vector = TargetPosition - clip.Position;
        direction = Maths.StepTo(target: Maths.Atan2(vector.Y, vector.X).SimplifyAngle(direction - (float)Math.PI), maxStep: angleStep * num * 30f, value: direction);
        Vector2 vector2 = VectorUtil.ToVector(speedValue * num, direction);
        clip.Position += vector2;
    }

    public void Remove()
    {
        clip.Visible = false;
        hasRemove = true;
    }
}
