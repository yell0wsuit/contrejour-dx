using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives;

public interface ISegmentedSpriteData<T> : IUpdatable where T : struct, IVertex
{
    int PairsCount { get; }

    bool IsDirty { get; }

    void FillLines(SegmentedSprite<T> sprite, List<Pair<T>> lines, ref Matrix matrix);
}
