using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Drawing;

public interface ITintVertex : IVertex, IVertexType
{
    float ColorRatio { get; set; }
}
