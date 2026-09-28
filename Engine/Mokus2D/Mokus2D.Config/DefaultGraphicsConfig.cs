using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Config;

public class DefaultGraphicsConfig : IGraphicsConfig
{
    public bool UseColorRatio => false;

    public ISpriteBatchEffect DefaultEffect { get; private set; }

    public DefaultGraphicsConfig()
    {
        DefaultEffect = new DefaultEffect(Mokus2DGame.Device);
    }

    public IQuad CreateDefaultQuad()
    {
        return new Quad<SpriteVertex>();
    }
}
