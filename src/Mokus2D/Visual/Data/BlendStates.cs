using System.Collections.Generic;

using Mokus2D.Graphics;

namespace Mokus2D.Visual.Data
{
    // Blend modes by the names animation files use.
    public static class BlendStates
    {
        private static readonly Dictionary<string, BlendMode> RegisteredBlendStates = new()
        {
            ["add"] = BlendMode.Additive
        };

        public static BlendMode GetByName(string name)
        {
            return RegisteredBlendStates[name];
        }
    }
}
