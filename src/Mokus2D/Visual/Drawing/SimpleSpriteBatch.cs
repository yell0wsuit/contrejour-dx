using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Drawing
{
    // Collects the quads of one texture and state, and draws them in one call on Flush.
    public class SimpleSpriteBatch(int spriteCount)
    {
        public const int DefaultSpriteCount = 2048;

        private Vertex[] _vertices = new Vertex[spriteCount * 4];

        private short[] _indices = new short[spriteCount * 6];

        private ITexture _texture;

        private SpriteBatchProperties _properties;

        private Vector2 _screenSize;

        private int _currentVertex;

        private int _currentIndex;

        public int TrianglesCount => _currentIndex / 3;

        public SimpleSpriteBatch()
            : this(DefaultSpriteCount)
        {
        }

        public void Begin(ITexture texture, Vector2 screenSize, SpriteBatchProperties properties)
        {
            _screenSize = screenSize;
            _texture = texture;
            _properties = properties;
            _currentVertex = 0;
            _currentIndex = 0;
            if (_texture.IsDisposed)
            {
                throw new ObjectDisposedException(Mokus2DGame.ContentManager.GetDisposedTextureName(_texture), "Texture is disposed.");
            }
        }

        public void DrawQuad(Quad quad)
        {
            SpriteBatchUtil.DrawQuad(quad, ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex);
        }

        public void Flush()
        {
            if (_currentVertex != 0)
            {
                SpriteBatchUtil.DrawTriangles(_screenSize, ref _properties, _texture, _vertices, _indices, _currentVertex, _currentIndex);
            }
        }
    }
}
