using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.PlatformSupport;

public static class GraphicsConfig
{
    public static ISimpleSpriteBatch<T> CreateSpriteBatch<T>(GraphicsDevice device) where T : struct, IVertex
    {
        return new SimpleSpriteBatch<T>(device, 2048);
    }
}
