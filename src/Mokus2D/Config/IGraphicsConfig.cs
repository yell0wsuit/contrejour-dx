using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;

namespace Mokus2D.Config
{
    public interface IGraphicsConfig
    {
        ISpriteBatchEffect DefaultEffect { get; }

        IQuad CreateDefaultQuad();
    }
}
