using Microsoft.Xna.Framework;

using Mokus2D.Config.Tint;
using Mokus2D.Visual.ShaderSupport.Parameters;

namespace Mokus2D.Visual.Shaders.Parallax
{
    public class ParallaxScrollerEffect : TintSpriteEffect
    {
        private readonly ShaderParameterFloat MainLayerScale;

        public ParallaxScrollerEffect()
            : this("Mokus2D.Shaders.Parallax.ParallaxScrollerShader")
        {
        }

        protected ParallaxScrollerEffect(string path)
            : base(path)
        {
            MainLayerScale = new ShaderParameterFloat(Parameters, "MainLayerScale");
            // Setting Value writes the shader parameter.
            _ = new ShaderParameterVector2(Parameters, "ViewPosition")
            {
                Value = Vector2.Zero
            };
            MainLayerScale.Value = 1f;
        }
    }
}
