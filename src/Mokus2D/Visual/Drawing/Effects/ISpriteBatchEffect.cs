using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Effects
{
    public interface ISpriteBatchEffect
    {
        void Apply(Matrix matrix, Texture2D texture);
    }
}
