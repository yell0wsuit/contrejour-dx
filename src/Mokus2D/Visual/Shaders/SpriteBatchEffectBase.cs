using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing.Effects;

namespace Mokus2D.Visual.Shaders
{
    public abstract class SpriteBatchEffectBase(string path) : ISpriteBatchEffect
    {
        protected Effect Effect { get; } = EffectUtil.LoadEffect(path);


        protected EffectParameterCollection Parameters => Effect.Parameters;

        public abstract void Apply(Matrix matrix, Texture2D texture);
    }
}
