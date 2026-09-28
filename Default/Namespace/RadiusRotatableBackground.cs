using Microsoft.Xna.Framework;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RadiusRotatableBackground : RotatableBackground
{
    protected Vector2 centerPosition;

    protected float radius;

    protected float rotation;

    public RadiusRotatableBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
        radius = 40f;
        centerPosition = node.Position;
        centerPosition.X += radius;
        rotationStep = 0.2f;
        rotation = 0f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        Vector2 vector = VectorUtil.ToVector(radius, MathHelper.ToRadians(node.RotationDegrees * 2f));
        node.Position = vector + centerPosition;
    }
}
