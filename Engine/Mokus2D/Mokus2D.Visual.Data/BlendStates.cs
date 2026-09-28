using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Data;

public static class BlendStates
{
	public static readonly BlendState NonPremultiplied;

	private static readonly Dictionary<string, BlendState> RegisteredBlendStates;

	static BlendStates()
	{
		RegisteredBlendStates = new Dictionary<string, BlendState>();
		NonPremultiplied = new BlendState();
		NonPremultiplied.ColorDestinationBlend = Blend.InverseSourceAlpha;
		NonPremultiplied.AlphaDestinationBlend = Blend.One;
		NonPremultiplied.ColorSourceBlend = Blend.SourceAlpha;
		NonPremultiplied.AlphaSourceBlend = Blend.One;
		RegisteredBlendStates.Add("add", BlendState.Additive);
	}

	public static BlendState GetByName(string name)
	{
		return RegisteredBlendStates[name];
	}
}
