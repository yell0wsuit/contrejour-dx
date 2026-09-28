using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class MenuPortal : Portal
{
    public MenuPortal(Vector2 position)
        : base(null, position)
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (Satellite part in parts)
        {
            part.Update(time);
        }
    }
}
