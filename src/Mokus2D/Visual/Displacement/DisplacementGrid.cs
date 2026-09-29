using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Displacement
{
    public class DisplacementGrid : SpriteBatchNode
    {
        public Size GridSize { get; }

        public Vector2 NodeSize { get; }

        protected Rectangle TextureRect { get; }

        private readonly SpriteVertex[] _vertices;

        private readonly Vector2[] _gridNodes;

        private readonly short[] _indices;

        private bool _positionsDirty;

        public Vector2 this[int w, int h]
        {
            get => _gridNodes[GetIndex(w, h)];
            set
            {
                short index = GetIndex(w, h);
                if (_gridNodes[index] != value)
                {
                    _gridNodes[index] = value;
                    _positionsDirty = true;
                }
            }
        }

        public DisplacementGrid(string name, Size gridSize)
            : this(Mokus2DGame.LoadResource<ISpriteData>(name), gridSize)
        {
        }

        public DisplacementGrid(ISpriteData data, Size gridSize)
            : this(data.Texture, data.TextureRect, gridSize, data.ScaleFactor)
        {
        }

        public DisplacementGrid(Texture2D texture, Size gridSize, float scaleFactor = 1f)
            : this(texture, texture.Bounds, gridSize, scaleFactor)
        {
        }

        public DisplacementGrid(Texture2D texture, Rectangle textureRect, Size gridSize, float scaleFactor = 1f)
            : base(texture)
        {
            Effect = new DefaultEffect(Mokus2DGame.Device);
            ResetDefaultEffect = true;
            ScaleFactor = scaleFactor;
            TextureRect = textureRect;
            GridSize = gridSize;
            NodeSize = textureRect.Size() / (gridSize - new Vector2(1f));
            int num = gridSize.Width * gridSize.Height;
            _vertices = new SpriteVertex[num];
            _gridNodes = new Vector2[num];
            _indices = new short[(gridSize.Width - 1) * (gridSize.Height - 1) * 6];
            InitializeGrid(texture, textureRect);
        }

        public void ResetTexture(Texture2D texture)
        {
            Texture = texture;
        }

        private void InitializeGrid(Texture2D texture, Rectangle textureRect)
        {
            InitializeIndices();
            Vector2 vector = new(texture.Width, texture.Height);
            Vector2 vector2 = new Vector2(TextureRect.X, textureRect.Y) / vector;
            Vector2 vector3 = GridSize - new Vector2(1f);
            Vector2 vector4 = TextureRect.Size() / (vector * vector3);
            Vector2 vector5 = TextureRect.Size() / vector3;
            for (int i = 0; i < GridSize.Width; i++)
            {
                for (int j = 0; j < GridSize.Height; j++)
                {
                    Vector2 vector6 = new(i, j);
                    Vector2 textureCoordinate = vector2 + (vector4 * vector6);
                    SetVertexData(i, j, Color.White, textureCoordinate);
                    this[i, j] = vector5 * vector6;
                }
            }
        }

        private void InitializeIndices()
        {
            for (int i = 0; i < GridSize.Width - 1; i++)
            {
                for (int j = 0; j < GridSize.Height - 1; j++)
                {
                    short index = GetIndex(i, j);
                    short index2 = GetIndex(i, j + 1);
                    short num = (short)(index + 1);
                    short num2 = (short)(index2 + 1);
                    int num3 = (i + (j * (GridSize.Width - 1))) * 6;
                    _indices[num3] = index;
                    _indices[num3 + 1] = num;
                    _indices[num3 + 2] = num2;
                    _indices[num3 + 3] = index;
                    _indices[num3 + 4] = num2;
                    _indices[num3 + 5] = index2;
                }
            }
        }

        private void SetVertexData(int w, int h, Color color, Vector2 textureCoordinate)
        {
            short index = GetIndex(w, h);
            _vertices[index].Color = color;
            _vertices[index].TextureCoordinate = textureCoordinate;
        }

        private short GetIndex(int w, int h)
        {
            return (short)(w + (h * GridSize.Width));
        }

        protected override void DrawSprite(VisualState state, Color color)
        {
            if (state.TransformationDirty || _positionsDirty)
            {
                RefreshPositions(state);
                _positionsDirty = false;
            }
            Drawer.Draw(_vertices, _indices);
        }

        private void RefreshPositions(VisualState state)
        {
            Matrix matrix = state.Matrix;
            for (int i = 0; i < _gridNodes.Length; i++)
            {
                Vector2.Transform(ref _gridNodes[i], ref matrix, out Vector2 result);
                _vertices[i].Position = new Vector3(result, 0f);
            }
        }
    }
}
