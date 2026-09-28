using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing.Effects;

namespace Mokus2D.Visual.Shaders;

public abstract class SpriteBatchEffectBase(string path) : ISpriteBatchEffect
{
    protected readonly Effect Effect = EffectUtil.LoadEffect(path);

    protected readonly string Path = path;

    protected EffectParameterCollection Parameters => Effect.Parameters;

    public abstract void Apply(Matrix matrix, Texture2D texture);
}
