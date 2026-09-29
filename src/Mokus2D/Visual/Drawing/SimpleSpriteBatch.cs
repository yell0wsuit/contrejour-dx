using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Drawing
{
    public class SimpleSpriteBatch<T> : ISimpleSpriteBatch<T>, ISimpleSpriteBatch where T : struct, IVertex
    {
        public const int DefaultSpriteCount = 2048;

        private T[] _vertices;

        protected ref T[] Vertices => ref _vertices;

        private readonly GraphicsDevice _device;

        private Texture2D _texture;

        private SpriteBatchProperties _properties;

        protected ref SpriteBatchProperties Properties => ref _properties;

        private int _verticesCount;

        private int _indicesCount;

        private int _currentVertex;

        protected ref int CurrentVertex => ref _currentVertex;

        private int _currentIndex;

        protected ref int CurrentIndex => ref _currentIndex;

        private readonly ISpriteBatchEffect _defaultEffect;

        private Vector2 _screenSize;

        private short[] _indices;

        protected ref short[] Indices => ref _indices;

        public int TrianglesCount => CurrentIndex / 3;

        public SimpleSpriteBatch(GraphicsDevice device)
            : this(device, 2048)
        {
        }

        public SimpleSpriteBatch(GraphicsDevice device, int defaultSpriteCount)
        {
            _device = device;
            _defaultEffect = CreateDefaultEffect();
            CreateBuffers(defaultSpriteCount);
        }

        protected virtual ISpriteBatchEffect CreateDefaultEffect()
        {
            return Mokus2DGame.Config.GraphicsConfig.DefaultEffect;
        }

        private void CreateBuffers(int spriteCount)
        {
            _verticesCount = spriteCount * 4;
            _indicesCount = spriteCount * 6;
            CreateBuffers(_verticesCount, _indicesCount);
        }

        protected virtual void CreateBuffers(int verticesCount, int indicesCount)
        {
            Vertices = new T[verticesCount];
            Indices = new short[indicesCount];
        }

        public void Begin(Texture2D texture, Vector2 screenSize, SpriteBatchProperties properties)
        {
            _screenSize = screenSize;
            _texture = texture;
            Properties = properties;
            CurrentVertex = 0;
            CurrentIndex = 0;
            if (_texture.IsDisposed)
            {
                throw new ObjectDisposedException(Mokus2DGame.ContentManager.GetDisposedTextureName(_texture), "Texture is disposed.");
            }
        }

        public void DrawQuad(Quad<T> quad)
        {
            SpriteBatchUtil.DrawQuad(quad, ref Vertices, ref Indices, ref CurrentVertex, ref CurrentIndex);
        }

        public virtual void Flush()
        {
            if (CurrentVertex != 0)
            {
                SpriteBatchUtil.DrawUserIndexedPrimitives(_device, _screenSize, ref Properties, _defaultEffect, _texture, Vertices, Indices, CurrentVertex, CurrentIndex);
            }
        }
    }
}
