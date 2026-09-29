using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Config.Tint;

public class TintGraphicsConfig : IGraphicsConfig
{
    public bool UseColorRatio { get; }

    public ISpriteBatchEffect DefaultEffect { get; private set; }

    public TintGraphicsConfig(bool tintEnabled = true)
    {
        UseColorRatio = tintEnabled;
        DefaultEffect = new TintSpriteEffect
        {
            TintEnabled = tintEnabled
        };
    }

    public IQuad CreateDefaultQuad()
    {
        return new TintQuad<TintSpriteVertex>();
    }
}
