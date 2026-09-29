using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Config.Defaults;
using Mokus2D.Config.Tint;
using Mokus2D.Content;
using Mokus2D.Visual.Data;

namespace Mokus2D.Config;

public class GameConfig
{
    public float AnimationFPS { get; set; } = 30f;

    public bool RenderTargetEnabled { get; set; } = true;

    public float MouseSpeed { get; set; } = 1f;

    public DebugConfig DebugConfig { get; } = new();

    public IGraphicsLoader GraphicsLoader { get; set; } = new SpriteLoaderCache(new OneFileResourcesLoader());
    public SpriteBatchProperties DefaultSpriteBatchProperties = new(BlendState.AlphaBlend, SamplerState.LinearClamp);

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
