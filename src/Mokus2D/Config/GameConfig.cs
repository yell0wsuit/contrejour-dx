using Mokus2D.Config.Defaults;
using Mokus2D.Content;
using Mokus2D.Graphics;
using Mokus2D.Visual.Data;

namespace Mokus2D.Config
{
    public class GameConfig
    {
        public float AnimationFPS { get; set; } = 30f;

        public float MouseSpeed { get; set; } = 1f;

        public DebugConfig DebugConfig { get; } = new();

        public IGraphicsLoader GraphicsLoader { get; set; } = new SpriteLoaderCache(new OneFileResourcesLoader());
        private SpriteBatchProperties defaultSpriteBatchProperties = new(BlendMode.AlphaBlend, SamplerMode.LinearClamp);

        public ref SpriteBatchProperties DefaultSpriteBatchProperties => ref defaultSpriteBatchProperties;
    }
}
