using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Config.Tint
{
    public class TintGraphicsConfig : IGraphicsConfig
    {
        public ISpriteBatchEffect DefaultEffect { get; private set; }

        public TintGraphicsConfig(bool tintEnabled = true)
        {
            DefaultEffect = new TintSpriteEffect
            {
                TintEnabled = tintEnabled
            };
        }

        public IQuad CreateDefaultQuad()
        {
            return new Quad<TintSpriteVertex>();
        }
    }
}
