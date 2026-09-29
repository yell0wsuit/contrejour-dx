using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Animation;

namespace Mokus2D.Visual.Shaders.Displacement
{
    public class DisplacementEffect : TextureMatrixEffectBase
    {
        private readonly EffectParameter _displacementTexture;

        private readonly EffectParameter _textureSize;

        public Texture2D DisplacementTexture { get; set; }

        public DisplacementEffect()
            : base("Mokus2D.Shaders.Displacement")
        {
            _displacementTexture = Parameters["DisplacementTexture"];
            _textureSize = Parameters["TextureSize"];
        }

        public override void Apply(Matrix matrix, Texture2D texture)
        {
            _displacementTexture.SetValue(DisplacementTexture);
            _textureSize.SetValue(texture.Size());
            base.Apply(matrix, texture);
        }
    }
}
