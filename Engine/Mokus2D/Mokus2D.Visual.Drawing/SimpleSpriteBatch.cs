using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Drawing;

public class SimpleSpriteBatch<T> : ISimpleSpriteBatch<T>, ISimpleSpriteBatch where T : struct, IVertex
{
    public const int DefaultSpriteCount = 2048;

    protected T[] _vertices;

    protected readonly GraphicsDevice _device;

    protected Texture2D _texture;

    protected SpriteBatchProperties _properties;

    private int _verticesCount;

    private int _indicesCount;

    protected int _currentVertex;

    protected int _currentIndex;

    private readonly ISpriteBatchEffect _defaultEffect;

    protected Vector2 _screenSize;

    public ISpriteBatchEffect _currentEffect;

    protected short[] _indices;

    public int TrianglesCount => _currentIndex / 3;

    public SimpleSpriteBatch(GraphicsDevice device)
        : this(device, 2048)
    {
    }

    public SimpleSpriteBatch(GraphicsDevice device, int defaultSpriteCount)
    {
        _device = device;
        _defaultEffect = CreateDefaultEffect();
        _currentEffect = _defaultEffect;
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
        _vertices = new T[verticesCount];
        _indices = new short[indicesCount];
    }

    public void Begin(Texture2D texture, Vector2 screenSize, SpriteBatchProperties properties, ISpriteBatchEffect effect)
    {
        _currentEffect = effect ?? _defaultEffect;
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

    public void Draw(T[] vertices)
    {
        SpriteBatchUtil.Draw(ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex, vertices);
    }

    public void Draw(T[] vertices, int verticesCount, short[] indices, int indicesCount)
    {
        SpriteBatchUtil.Draw(ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex, vertices, verticesCount, indices, indicesCount);
    }

    public void DrawQuad(Quad<T> quad)
    {
        SpriteBatchUtil.DrawQuad(quad, ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex);
    }

    public virtual void Flush()
    {
        if (_currentVertex != 0)
        {
            SpriteBatchUtil.DrawUserIndexedPrimitives(_device, _screenSize, ref _properties, _currentEffect, _texture, _vertices, _indices, _currentVertex, _currentIndex);
        }
    }
}
