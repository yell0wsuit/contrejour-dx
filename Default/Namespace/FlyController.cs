using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class FlyController : FlyBase
{
    protected const float MIN_VERTICAL_OFFSET = -5f;

    protected const float MAX_VERTICAL_OFFSET = 20f;

    protected const float MAX_HORIZONTAL_OFFSET = 20f;

    protected IWindManager windProvider;

    protected IGrassControllerContainer grassControllerContainer;

    protected float horizontalOffset;

    protected float initialGroundY;

    protected Vector2 scareOffset;

    protected float scareTime;

    protected int scared;

    protected float verticalOffset;

    protected float verticalOffsetMultiplier;

    protected float windOffset;

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
    public FlyController(IWindManager _windProvider, IGrassControllerContainer _grassControllerContainer, Particle _particle, float? scale = null, float? windOffsetRange = null)
        : base(_particle, scale ?? Maths.Random(0.8f, 1.2f))
    {
        windProvider = _windProvider;
        initialGroundY = -1f;
        grassControllerContainer = _grassControllerContainer;
        scareOffset = new Vector2(Maths.Random(10f, 30f), Maths.Random(30f, 50f));
        scared = 0;
        scareTime = 0f;
        windOffset = Maths.Random((0f - windOffsetRange) ?? (-0.5f), windOffsetRange ?? 0.5f);
        horizontalOffset = Maths.Random(10f, 20f);
        verticalOffsetMultiplier = Maths.Random(1f, 2f);
        verticalOffset = Maths.Random(-5f, 20f);
    }

    public void Scare(int direction)
    {
        if (scared == 0)
        {
            scared = direction;
        }
        scareTime = Maths.Random(0.5f, 2.5f);
    }

    public override void Update(float time)
    {
        if (initialGroundY == -1f)
        {
            initialGroundY = grassControllerContainer.GrassController.Y;
        }
        targetPosition = ChooseTarget();
        if (scared != 0)
        {
            targetPosition.X += scared * scareOffset.X;
            targetPosition.Y += scareOffset.Y;
            stepY = Math.Abs(particle.Position.Y - targetPosition.Y) / scareOffset.Y;
            scareTime -= time;
            if (scareTime <= 0f)
            {
                Unscare();
            }
        }
        base.Update(time);
    }

    private Vector2 ChooseTarget()
    {
        return new Vector2(initialPosition.X + (horizontalOffset * windProvider.WindManager.GetWind(windOffset)), initialPosition.Y + (verticalOffset * Maths.Sin(verticalStep)) + (grassControllerContainer.GrassController.Y - initialGroundY));
    }

    public void Unscare()
    {
        scared = 0;
    }
}
