using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class Satellite : IUpdatable, IRemovable
{
    private const float MAX_STEP = 0.1f;

    private const float MIN_STEP = 0.05f;

    protected float direction;

    protected float speedValue;

    protected float angleStep;

    protected bool hasRemove;

    protected Particle clip;

    protected ContreJourGame game;

    protected Vector2 initialPosition;

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

    public Satellite(ContreJourGame _game, Particle _clip, BodyClip parent, float _direction, Vector2 position)
    {
        target = parent;
        initialPosition = position;
        clip = _clip;
        speedValue = Maths.Random(15f, 25f) * 2f;
        angleStep = Maths.Random(0.18f, 0.28f);
        direction = _direction;
        clip.Position = position;
        if (_game != null)
        {
            game = _game;
            game.AddUpdatable(this);
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
