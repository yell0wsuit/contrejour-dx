using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.ShaderSupport;

public class GridNodeCell : Node
{
    public readonly Point CellIndex;

    public GridNodeCell(Point cellIndex)
    {
        CellIndex = cellIndex;
    }
}
