using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors;

public class PolygonProcessor(PhysicsConstructor constructor, Vector2[] coords) : ShapeProcessor(constructor)
{
    private readonly Vector2[] _coords = coords;

    private readonly Vertices _resultCoords = new Vertices(coords.Length);

    public override Shape Process(Node item, Vector2 positionOffset)
    {
        _resultCoords.Clear();
        Vector2[] coords = _coords;
        foreach (Vector2 position in coords)
        {
            _resultCoords.Add(Constructor.ToPhysics(position, item, positionOffset));
        }
        if (VectorUtil.WherePoint(_resultCoords[0], _resultCoords[1], _resultCoords[2]) < 0f)
        {
            _resultCoords.Reverse();
        }
        return new PolygonShape(_resultCoords, Constructor.GetDensity());
    }
}
