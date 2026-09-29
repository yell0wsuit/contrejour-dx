using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Graphics;
using Mokus2D.PlatformSupport;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing
{
    public class BatchSelector : IDrawer
    {
        private bool _batchStarted;

        private SpriteBatchProperties _currentBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

        private ITexture _currentTexture;

        private ISimpleSpriteBatch _currentSpriteBatch;

        private Vector2 _currentScreenSize;

        private readonly Dictionary<Type, ISimpleSpriteBatch> _batches = [];

        private bool _spriteBatchDirty = true;

        public int DrawCallsCount { get; private set; }

        public int TrianglesDrawnCount { get; private set; }

        public int NodesDrawnCount { get; private set; }

        public void BeginBatch(ITexture texture, SpriteBatchProperties properties)
        {
            if (_currentTexture != texture || _currentBatchProperties != properties)
            {
                _spriteBatchDirty = true;
                _currentTexture = texture;
                _currentBatchProperties = properties;
            }
        }

        private void StartDraw<T>() where T : struct, IVertex
        {
            if (_spriteBatchDirty)
            {
                EndDraw();
                _spriteBatchDirty = false;
            }
            if (!_batchStarted)
            {
                _batchStarted = true;
                _currentSpriteBatch = _batches.GetValueOrDefault(typeof(T));
                if (_currentSpriteBatch == null)
                {
                    _currentSpriteBatch = GraphicsConfig.CreateSpriteBatch<T>(Mokus2DGame.Device);
                    _batches.Add(typeof(T), _currentSpriteBatch);
                }
                _currentSpriteBatch.Begin(_currentTexture, _currentScreenSize, _currentBatchProperties);
            }
        }

        private ISimpleSpriteBatch<T> GetSpriteBatch<T>() where T : struct, IVertex
        {
            return (ISimpleSpriteBatch<T>)_currentSpriteBatch;
        }

        public void Draw<T>(Quad<T> quad) where T : struct, IVertex
        {
            StartDraw<T>();
            GetSpriteBatch<T>().DrawQuad(quad);
        }

        public void Reset(Vector2 screenSize)
        {
            _currentScreenSize = screenSize;
            DrawCallsCount = 0;
            TrianglesDrawnCount = 0;
        }

        public void EndDraw()
        {
            if (_batchStarted)
            {
                _currentSpriteBatch.Flush();
                _batchStarted = false;
                DrawCallsCount++;
                TrianglesDrawnCount += _currentSpriteBatch.TrianglesCount;
            }
        }

        public void IncreaseNodesDrawnCount()
        {
            NodesDrawnCount++;
        }
    }
}
