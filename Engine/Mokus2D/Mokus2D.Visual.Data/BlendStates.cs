using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Data;

public static class BlendStates
{
    public static readonly BlendState NonPremultiplied;

    private static readonly Dictionary<string, BlendState> RegisteredBlendStates;

    static BlendStates()
    {
        RegisteredBlendStates = [];
        NonPremultiplied = new BlendState
        {
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            AlphaDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.One
        };
        RegisteredBlendStates.Add("add", BlendState.Additive);
    }

    public static BlendState GetByName(string name)
    {
        return RegisteredBlendStates[name];
    }
}
