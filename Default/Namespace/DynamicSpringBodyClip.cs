using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class DynamicSpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config) : SpringBodyClip(builder, body, clip, config)
{
    private Vector2 relativePosition;

    private Vector2 worldPosition;

    private Vector2 oldPosition;

    private float oldAngle;

    private float oldAngleForSticked;

    private bool TransformChanged => oldPosition != Body.Position || oldAngle != Body.Rotation;

    protected override void CreateShadow()
    {
    }

    private void RefreshPositions()
    {
        relativePosition = Body.GetLocalPoint(sticked.Body.Position);
        worldPosition = sticked.Body.Position;
        oldAngleForSticked = Body.Rotation;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (TransformChanged)
        {
            RefreshPoints();
            oldPosition = Body.Position;
            oldAngle = Body.Rotation;
        }
    }

    protected override void UpdateSticked()
    {
        if (TransformChanged)
        {
            Vector2 vector = Body.GetWorldPoint(relativePosition) - worldPosition;
            float num = Body.Rotation - oldAngleForSticked;
            sticked.Body.SetTransform(sticked.Body.Position + vector, sticked.Body.Rotation + num);
            RefreshPoints();
            sticked.UpdatePosition();
        }
        base.UpdateSticked();
        RefreshPositions();
    }

    protected override void SetSticked(ILaunchable value)
    {
        base.SetSticked(value);
        if (value != null)
        {
            RefreshPositions();
        }
    }
}
