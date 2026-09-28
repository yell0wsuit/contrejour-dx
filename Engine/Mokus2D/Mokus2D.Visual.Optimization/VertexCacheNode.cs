using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Optimization;

public class VertexCacheNode<TVertex> : Node, IDrawer where TVertex : struct, IVertex
{
    private const int DefaultVerticesCount = 512;

    private static readonly VisualState OneState = new VisualState
    {
        TransformationDirty = true
    };

    private short[] _indices = new short[512];

    private TVertex[] _vertices = new TVertex[512];

    private int _currentVertex;

    private int _currentIndex;

    private int _drawnNodes;

    private IDrawer _parentDrawer;

    private SpriteBatchProperties _spriteBatchProperties = Mokus2DGame.Config.DefaultSpriteBatchProperties;

    private Matrix _childrenMatrix;

    private Texture2D _texture;

    private VertexBuffer _vertexBuffer;

    private IndexBuffer _indexBuffer;

    public BlendState Blend
    {
        get
        {
            return _spriteBatchProperties.Blend;
        }
        set
        {
            _spriteBatchProperties.Blend = value;
        }
    }

    public SamplerState SamplerState
    {
        get
        {
            return _spriteBatchProperties.SamplerState;
        }
        set
        {
            _spriteBatchProperties.SamplerState = value;
        }
    }

    public VertexCacheNode()
    {
        base.Effect = Mokus2DGame.Config.GraphicsConfig.DefaultEffect;
        ResetDefaultEffect = true;
        UpdateChildrenTransformations = false;
        base.TransformationsRefreshedEvent += OnTransformationsRefreshed;
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
        _childrenMatrix = base.CompositeState.GetCombinedScreenMatrix(base.Root.Size);
    }

    public void RefreshVerticesCache()
    {
        _currentVertex = 0;
        _currentIndex = 0;
        UpdateChildrenTransformations = true;
        _drawnNodes = 0;
        RefreshChildrenTransformations(OneState, this);
        int index = DrawChildrenPart(0, positiveLayers: false);
        DrawChildrenPart(index, positiveLayers: true);
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
            SpriteBatchUtil.DrawIndexedPrimitives(Mokus2DGame.Device, ref _childrenMatrix, ref _spriteBatchProperties, base.Effect, _texture, _vertexBuffer, _indexBuffer, _currentVertex, _currentIndex);
        }
    }

    protected override void DrawWithChildren()
    {
        if (DrawSelf)
        {
            Draw(base.CompositeState);
            Drawer.IncreaseNodesDrawnCount();
        }
    }

    public void StartEffect(ISpriteBatchEffect effect)
    {
        if (effect != null && effect != base.Effect)
        {
            throw new Exception("Effect of cached nodes must be the same");
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
            throw new Exception("Texture reset is not allowed");
        }
    }

    public void EndDraw()
    {
        throw new Exception("EndDraw not allowed");
    }

    public void IncreaseNodesDrawnCount()
    {
        _drawnNodes++;
    }

    [Conditional("DEBUG")]
    private void CheckType(Type type)
    {
        if ((object)typeof(TVertex) != type)
        {
            throw new Exception("Type of vertex doesn't match");
        }
    }
}
