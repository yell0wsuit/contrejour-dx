using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Effects;

public class DefaultEffect : BasicEffect, ISpriteBatchEffect
{
    public DefaultEffect(GraphicsDevice device)
        : base(device)
    {
        base.World = Matrix.Identity;
        base.View = Matrix.Identity;
        base.Alpha = 1f;
        base.VertexColorEnabled = true;
        base.TextureEnabled = true;
        base.VertexColorEnabled = true;
    }

    public void Apply(Matrix matrix, Texture2D texture)
    {
        base.Projection = matrix;
        base.Texture = texture;
        Mokus2DGame.Device.Textures[0] = texture;
        base.CurrentTechnique.Passes[0].Apply();
    }
}
