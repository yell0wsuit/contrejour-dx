using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Optimization
{
    public class VertexCacheNode<TVertex> : Node, IDrawer where TVertex : struct, IVertex
    {
        private static readonly VisualState OneState = new()
        {
            TransformationDirty = true
        };

        private short[] _indices = new short[512];

        private TVertex[] _vertices = new TVertex[512];

        private int _currentVertex;

        private int _currentIndex;

        private IDrawer _parentDrawer;

        private SpriteBatchProperties _spriteBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

        private Matrix _childrenMatrix;

        private Texture2D _texture;

        private VertexBuffer _vertexBuffer;

        private IndexBuffer _indexBuffer;

        public BlendState Blend
        {
            get => _spriteBatchProperties.Blend;
            set => _spriteBatchProperties.Blend = value;
        }

        public SamplerState SamplerState
        {
            get => _spriteBatchProperties.SamplerState;
            set => _spriteBatchProperties.SamplerState = value;
        }

        public VertexCacheNode()
        {
            Effect = Mokus2DGame.Config.GraphicsConfig.DefaultEffect;
            ResetDefaultEffect = true;
            UpdateChildrenTransformations = false;
            TransformationsRefreshedEvent += OnTransformationsRefreshed;
        }

        internal override void SetRootAndDrawer(RootNode root, IDrawer drawer)
        {
            _parentDrawer = drawer;
            base.SetRootAndDrawer(root, this);
        }

        public void ResetSize()
        {
            SetTransformationDirty();
        }

        private void OnTransformationsRefreshed()
        {
            _childrenMatrix = CompositeState.GetCombinedScreenMatrix(Root.Size);
        }

        public void RefreshVerticesCache()
        {
            _currentVertex = 0;
            _currentIndex = 0;
            UpdateChildrenTransformations = true;
            RefreshChildrenTransformations(OneState, this);
            int index = DrawChildrenPart(0, positiveLayers: false);
            _ = DrawChildrenPart(index, positiveLayers: true);
            UpdateChildrenTransformations = false;
            if (_currentVertex > 0)
            {
                _vertexBuffer = new VertexBuffer(Mokus2DGame.Device, typeof(TVertex), _currentVertex, BufferUsage.None);
                _indexBuffer = new IndexBuffer(Mokus2DGame.Device, typeof(short), _currentIndex, BufferUsage.None);
                _vertexBuffer.SetData(_vertices, 0, _currentVertex);
                _indexBuffer.SetData(_indices, 0, _currentIndex);
            }
        }

        private void RefreshTransformations(VisualState state, Node node)
        {
            if (TransformationUtil.ShouldRefreshNode(node))
            {
                node.RefreshVisualState(state);
                RefreshChildrenTransformations(node.CompositeState, node);
            }
        }

        private void RefreshChildrenTransformations(VisualState state, Node node)
        {
            foreach (Node child in node.Children)
            {
                RefreshTransformations(state, child);
            }
        }

        public override void Draw(VisualState state)
        {
            if (_currentVertex > 0)
            {
                _parentDrawer.EndDraw();
                SpriteBatchUtil.DrawIndexedPrimitives(Mokus2DGame.Device, ref _childrenMatrix, ref _spriteBatchProperties, Effect, _texture, _vertexBuffer, _indexBuffer, _currentVertex, _currentIndex);
            }
        }

        protected override void DrawWithChildren()
        {
            if (DrawSelf)
            {
                Draw(CompositeState);
                Drawer.IncreaseNodesDrawnCount();
            }
        }

        public void StartEffect(ISpriteBatchEffect effect)
        {
            if (effect != null && effect != Effect)
            {
                throw new InvalidOperationException("Effect of cached nodes must be the same");
            }
        }

        public void Draw<T>(Quad<T> quad) where T : struct, IVertex
        {
            SpriteBatchUtil.DrawQuad((Quad<TVertex>)(object)quad, ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex);
        }

        public void Draw<T>(T[] vertices) where T : struct, IVertex
        {
            SpriteBatchUtil.Draw(ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex, (TVertex[])(object)vertices);
        }

        public void Draw<T>(T[] vertices, int verticesCount, short[] indices, int indicesCount) where T : struct, IVertex
        {
            SpriteBatchUtil.Draw(ref _vertices, ref _indices, ref _currentVertex, ref _currentIndex, (TVertex[])(object)vertices, verticesCount, indices, indicesCount);
        }

        public void BeginBatch(Texture2D texture, SpriteBatchProperties properties)
        {
            if (_texture == null)
            {
                _texture = texture;
            }
            else if (_texture != texture)
            {
                throw new NotSupportedException("Texture reset is not allowed");
            }
        }

        public void EndDraw()
        {
            throw new NotSupportedException("EndDraw not allowed");
        }

        public void IncreaseNodesDrawnCount()
        {
        }
    }
}
