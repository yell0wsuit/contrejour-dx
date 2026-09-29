using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Config.Defaults;
using Mokus2D.Config.Tint;
using Mokus2D.Content;
using Mokus2D.Visual.Data;

namespace Mokus2D.Config
{
    public class GameConfig
    {
        public float AnimationFPS { get; set; } = 30f;

        public float MouseSpeed { get; set; } = 1f;

        public DebugConfig DebugConfig { get; } = new();

        public IGraphicsLoader GraphicsLoader { get; set; } = new SpriteLoaderCache(new OneFileResourcesLoader());
        private SpriteBatchProperties defaultSpriteBatchProperties = new(BlendState.AlphaBlend, SamplerState.LinearClamp);

        public ref SpriteBatchProperties DefaultSpriteBatchProperties => ref defaultSpriteBatchProperties;

        public IGraphicsConfig GraphicsConfig
        {
            get
            {
                field ??= new TintGraphicsConfig();
                return field;
            }

            set;
        }
    }
}
