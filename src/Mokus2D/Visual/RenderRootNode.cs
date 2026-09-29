using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Util;

namespace Mokus2D.Visual
{
    public class RenderRootNode : RootNode
    {
        private Color ClearColor = Color.Black * 0f;

        public RenderTarget2D RenderTarget { get; private set; }

        public RenderRootNode(Vector2 size)
            : this((int)size.X, (int)size.Y)
        {
        }

        public RenderRootNode(int width, int height, Vector2 spritesScaleFactor)
            : base(width, height, spritesScaleFactor)
        {
            RenderTarget = GraphicsUtil.CreateRenderTarget(width, height);
        }

        public RenderRootNode(int width, int height)
            : this(GraphicsUtil.CreateRenderTarget(width, height))
        {
        }

        public RenderRootNode(RenderTarget2D renderTarget)
            : base(renderTarget.Width, renderTarget.Height)
        {
            RenderTarget = renderTarget;
        }

        public override void ResetSize(Vector2 size)
        {
            base.ResetSize(size);
            RenderTarget = GraphicsUtil.CreateRenderTarget((int)size.X, (int)size.Y);
        }

        public void UpdateAndDraw(float time)
        {
            UpdateNode(time);
            DrawAll();
        }

        public LayerColor AddDebugLayer(string whiteRect)
        {
            return AddDebugLayer(whiteRect, Color.Green);
        }

        public LayerColor AddDebugLayer(string whiteRect, Color color)
        {
            LayerColor layerColor = new(color, whiteRect)
            {
                OpacityFloat = 0.5f
            };
            AddChild(layerColor, int.MaxValue);
            return layerColor;
        }

        public override void DrawAll()
        {
            Mokus2DGame.Device.SetRenderTarget(RenderTarget);
            Mokus2DGame.Device.Clear(ClearColor);
            base.DrawAll();
            Mokus2DGame.Device.SetRenderTarget(null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RenderTarget.Dispose();
        }
    }
}
