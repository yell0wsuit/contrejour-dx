using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RadiusRotatableBackground : RotatableBackground
{
    protected Vector2 centerPosition;

    private readonly float radius;

    public RadiusRotatableBackground(Node node, Hashtable config, ContreJourGame game)
        : base(node, config, game)
    {
        radius = 40f;
        centerPosition = this.Node.Position;
        centerPosition.X += radius;
        RotationStep = 0.2f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        Vector2 vector = VectorUtil.ToVector(radius, MathHelper.ToRadians(Node.RotationDegrees * 2f));
        Node.Position = vector + centerPosition;
    }
}
