using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RadiusRotatableBackground : RotatableBackground
{
    private Vector2 centerPosition;

    protected ref Vector2 CenterPosition => ref centerPosition;

    private readonly float radius;

    public RadiusRotatableBackground(Node node, Hashtable config, ContreJourGame game)
        : base(node, config, game)
    {
        radius = 40f;
        CenterPosition = this.Node.Position;
        CenterPosition.X += radius;
        RotationStep = 0.2f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        Vector2 vector = VectorUtil.ToVector(radius, MathHelper.ToRadians(Node.RotationDegrees * 2f));
        Node.Position = vector + CenterPosition;
    }
}
