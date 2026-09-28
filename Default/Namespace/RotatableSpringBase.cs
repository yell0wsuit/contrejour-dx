using Mokus2D.Visual;

namespace Default.Namespace;

public abstract class RotatableSpringBase(LevelBuilderBase builder, object body, Node clip, Hashtable config) : DynamicSpringBodyClip(builder, body, clip, config)
{
    protected abstract bool IsMoving { get; }

    public override void Update(float time)
    {
        if (IsMoving)
        {
            RefreshSmokeAngle();
        }
        base.Update(time);
    }

    protected override void CreateShadow()
    {
    }
}
