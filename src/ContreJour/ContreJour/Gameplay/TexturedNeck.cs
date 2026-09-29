using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace ContreJour.Gameplay
{
    public class TexturedNeck : SpriteBatchNode
    {
        private readonly List<Vector2> vertices = [];

        private readonly List<Vector2> textureCoords = [];

        public TexturedNeck(string textureFile)
        {
            Texture = ClipFactory.GetTexture(textureFile);
        }

        public void AddPoint(Vector2 point)
        {
            vertices.Add(point);
            RefreshTextureCoords();
        }

        public void RefreshTextureCoords()
        {
            for (int i = textureCoords.Count; i < vertices.Count; i++)
            {
                Vector2 item = new(i / 2, i % 2);
                textureCoords.Add(item);
            }
        }

        public void ClearVertices()
        {
            vertices.Clear();
        }

        public void Clear()
        {
            vertices.Clear();
            textureCoords.Clear();
        }

        protected override void DrawSprite(VisualState state, Color color)
        {
            _ = vertices.Count;
            _ = 2;
        }
    }
}
