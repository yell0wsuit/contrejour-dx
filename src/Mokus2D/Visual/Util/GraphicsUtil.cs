using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Platforms.Visual;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Util
{
    public static class GraphicsUtil
    {
        public static void ClearRenderTarget(RenderTarget2D renderTarget)
        {
            Mokus2DGame.Device.SetRenderTarget(renderTarget);
            Mokus2DGame.Device.Clear(Color.Black * 0f);
            Mokus2DGame.Device.SetRenderTarget(null);
        }

        public static RenderTarget2D CreateRenderTarget(Vector2 size)
        {
            return new RenderTarget2D(Mokus2DGame.Device, PlatformRenderUtil.GetRenderTargetSize(size.X), PlatformRenderUtil.GetRenderTargetSize(size.Y), mipMap: false, SurfaceFormat.Color, DepthFormat.None);
        }

        public static RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return CreateRenderTarget(new Vector2(width, height));
        }

        public static void PositionLine(ISizeNode line, Vector2 from, Vector2 to)
        {
            PositionLine(line, from, to, 0f);
        }

        public static void PositionLine(ISizeNode line, Vector2 from, Vector2 to, float offset)
        {
            PositionLine((Node)line, from, to, line.Size.X, offset);
        }

        public static void PositionLine(Node line, Vector2 from, Vector2 to, float nodeWidth, float offset)
        {
            line.Position = from;
            float num = Vector2.Distance(to, line.Position);
            line.ScaleX = (num + offset) / nodeWidth;
            line.RotationRadians = (to - line.Position).Atan2();
        }
    }
}
