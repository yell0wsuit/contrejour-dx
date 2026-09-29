using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.ShaderSupport
{
    public class GridNodeCell(Point cellIndex) : Node
    {
        public Point CellIndex { get; } = cellIndex;
    }
}
