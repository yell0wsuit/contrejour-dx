using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class MovableSpringBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config) : DynamicSpringBodyClip(_builder, _body, _clip, _config)
{
    protected DragableBodyClip mover;

    protected Vector2 offset;

    protected Vector2 moverPosition;

    public override void Update(float time)
    {
        if (mover == null)
        {
            mover = (DragableBodyClip)FarseerUtil.Query(builder.World, Body.Position, 3.3333333f, typeof(DragableBodyClip));
            offset = Body.Position - mover.Body.Position;
            moverPosition = mover.Body.Position;
            Body.BodyType = (BodyType)1;
        }
        else if (moverPosition != mover.Body.Position)
        {
            Vector2 vector = mover.Body.Position + offset;
            Body.SetTransform(vector, Body.Rotation);
            moverPosition = mover.Body.Position;
        }
        base.Update(time);
    }

    protected override void CreateShadow()
    {
    }
}
