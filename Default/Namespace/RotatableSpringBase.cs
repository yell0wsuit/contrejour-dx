using Mokus2D.Visual;

namespace Default.Namespace;

public abstract class RotatableSpringBase : DynamicSpringBodyClip
{
    protected abstract bool IsMoving { get; }

    public RotatableSpringBase(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
    }

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
