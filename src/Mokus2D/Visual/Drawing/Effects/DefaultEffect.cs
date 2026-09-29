using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Effects
{
    public class DefaultEffect : BasicEffect, ISpriteBatchEffect
    {
        public DefaultEffect(GraphicsDevice device)
            : base(device)
        {
            World = Matrix.Identity;
            View = Matrix.Identity;
            Alpha = 1f;
            VertexColorEnabled = true;
            TextureEnabled = true;
            VertexColorEnabled = true;
        }

        public void Apply(Matrix matrix, Texture2D texture)
        {
            Projection = matrix;
            Texture = texture;
            Mokus2DGame.Device.Textures[0] = texture;
            CurrentTechnique.Passes[0].Apply();
        }
    }
}
