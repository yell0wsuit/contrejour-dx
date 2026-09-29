using Microsoft.Xna.Framework;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Parallel;

namespace Mokus2D.Visual
{
    public class RootNode : Node
    {
        private readonly BatchSelector _batchSelector;

        private readonly OneThreadTransformCalculator _transformCalculator;

        private readonly bool DrawEnabled = true;

        internal VisualState RootState;

        public int NodesDrawnCount => _batchSelector.NodesDrawnCount;

        public Vector2 Size { get; private set; }

        public Vector2 SpritesScaleFactor
        {
            get => RootState.SpritesScaleFactor;
            set => RootState.SpritesScaleFactor = value;
        }

        public static void ResetUpdateThread()
        {
        }

        public RootNode(int width, int height, Vector2 spritesScaleFactor)
        {
            ResetUpdateThread();
            Root = this;
            _transformCalculator = new OneThreadTransformCalculator(this);
            _batchSelector = Mokus2DGame.BatchSelector;
            Drawer = _batchSelector;
            Size = new Vector2(width, height);
            RootState = new VisualState(spritesScaleFactor);
            CompositeState = new VisualState(RootState);
        }

        public RootNode(int width, int height)
            : this(width, height, Vector2.One)
        {
        }

        public RootNode(Vector2 size)
            : this((int)size.X, (int)size.Y, Vector2.One)
        {
        }

        public RootNode(Vector2 size, Vector2 spritesScaleFactor)
            : this((int)size.X, (int)size.Y, spritesScaleFactor)
        {
        }

        public virtual void ResetSize(Vector2 size)
        {
            Size = size;
        }

        public override Vector2 GlobalToLocal(Vector2 source, bool refreshTransformations = true)
        {
            Matrix matrix = Matrix.Invert(NodeMatrix);
            return Vector2.Transform(source, matrix);
        }

        public virtual void DrawAll()
        {
            if (DrawEnabled)
            {
                _transformCalculator.DoTransformations();
                _batchSelector.Reset(Size);
                DrawNode();
                _batchSelector.EndDraw();
            }
        }
    }
}
