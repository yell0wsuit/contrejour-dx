using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Primitives.Collections
{
    public class VerticesAndIndices<T> where T : IVertex
    {
        private readonly Pow2Array<T> _vertices = new(128);

        private readonly Pow2Array<short> _indices = new(128);

        public void Clear()
        {
            _vertices.Clear();
            _indices.Clear();
        }
    }
}
