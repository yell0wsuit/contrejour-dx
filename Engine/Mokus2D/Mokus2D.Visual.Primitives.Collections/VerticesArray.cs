using Microsoft.Xna.Framework;

using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives.Collections;

public class VerticesArray<T> : Pow2Array<T> where T : struct, IVertex
{
    public void FillColor(Color color)
    {
        for (int i = 0; i < Length; i++)
        {
            Items[i].Color = color;
        }
    }
}
