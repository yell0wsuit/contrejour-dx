using Microsoft.Xna.Framework;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing
{
    public class BatchSelector : IDrawer
    {
        private bool _batchStarted;

        private SpriteBatchProperties _currentBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

        private ITexture _currentTexture;

        private SimpleSpriteBatch _spriteBatch;

        private Vector2 _currentScreenSize;

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

        private void StartDraw()
        {
            if (_spriteBatchDirty)
            {
                EndDraw();
                _spriteBatchDirty = false;
            }
            if (!_batchStarted)
            {
                _batchStarted = true;
                _spriteBatch ??= new SimpleSpriteBatch();
                _spriteBatch.Begin(_currentTexture, _currentScreenSize, _currentBatchProperties);
            }
        }

        public void Draw(Quad quad)
        {
            StartDraw();
            _spriteBatch.DrawQuad(quad);
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
                _spriteBatch.Flush();
                _batchStarted = false;
                DrawCallsCount++;
                TrianglesDrawnCount += _spriteBatch.TrianglesCount;
            }
        }

        public void IncreaseNodesDrawnCount()
        {
            NodesDrawnCount++;
        }
    }
}
