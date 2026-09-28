using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing.Effects;

namespace Mokus2D.Visual.Shaders;

public abstract class SpriteBatchEffectBase : ISpriteBatchEffect
{
    protected readonly Effect Effect;

    protected readonly string Path;

    protected EffectParameterCollection Parameters => Effect.Parameters;

    protected SpriteBatchEffectBase(string path)
    {
        Path = path;
        Effect = EffectUtil.LoadEffect(path);
    }

    public abstract void Apply(Matrix matrix, Texture2D texture);
}
