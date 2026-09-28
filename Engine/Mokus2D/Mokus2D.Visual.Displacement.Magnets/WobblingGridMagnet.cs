using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets;

public class WobblingGridMagnet : GridMagnetBase
{
    public float PhaseOffset = 1f;
    private readonly Vector2 _nodeSize;

    private readonly CosChanger _verticalOffset;

    private readonly CosChanger _horizontalOffset;

    public float Step
    {
        get;
        set
        {
            field = value;
            _verticalOffset.Step = value;
            _horizontalOffset.Step = value;
        }
    } = 2f;

    public WobblingGridMagnet(Vector2 size, Vector2 nodeSize)
        : base(size)
    {
        _nodeSize = nodeSize;
        _verticalOffset = new CosChanger(-1f, 1f, 0.71428573f * Step);
        _horizontalOffset = new CosChanger(-1f, 1f, Step);
    }

    public override void Update(float time)
    {
        base.Update(time);
        _verticalOffset.Update(time);
        _horizontalOffset.Update(time);
    }

    public override Vector2 GetForce(Vector2 relativePosition)
    {
        Vector2 vector = (relativePosition / _nodeSize).ToIntVector();
        return new Vector2
        {
            X = _horizontalOffset.GetValue(vector.Y * PhaseOffset) * Power,
            Y = _verticalOffset.GetValue(vector.X * PhaseOffset) * Power
        };
    }
}
