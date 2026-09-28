using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Config.Defaults;
using Mokus2D.Config.Tint;
using Mokus2D.Content;
using Mokus2D.Visual.Data;

namespace Mokus2D.Config;

public class GameConfig
{
	public float AnimationFPS = 30f;

	public bool RenderTargetEnabled = true;

	public float MouseSpeed = 1f;

	public readonly DebugConfig DebugConfig = new DebugConfig();

	private SpriteBatchProperties? _defaultSpriteBatchProperties;

	public IGraphicsLoader GraphicsLoader = new SpriteLoaderCache(new OneFileResourcesLoader());

	private IGraphicsConfig _graphicsConfig;

	public SpriteBatchProperties DefaultSpriteBatchProperties = new SpriteBatchProperties(BlendState.AlphaBlend, SamplerState.LinearClamp);

	public IGraphicsConfig GraphicsConfig
	{
		get
		{
			if (_graphicsConfig == null)
			{
				_graphicsConfig = new TintGraphicsConfig();
			}
			return _graphicsConfig;
		}
		set
		{
			_graphicsConfig = value;
		}
	}
}
