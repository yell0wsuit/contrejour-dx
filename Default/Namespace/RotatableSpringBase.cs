using Mokus2D.Visual;

namespace Default.Namespace;

public abstract class RotatableSpringBase(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config) : DynamicSpringBodyClip(_builder, _body, _clip, _config)
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
