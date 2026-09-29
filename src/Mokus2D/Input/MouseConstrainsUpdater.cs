using System;

using Mokus2D.Util;
using Mokus2D.Util.Data;

namespace Mokus2D.Input
{
    public class MouseConstrainsUpdater
    {
        private readonly Flag _applied = new(on: false);

        protected virtual bool ShouldApplyConstrains => Mokus2DGame.Instance.AcceptsInput && Mokus2DGame.Instance.IsFullScreen;

        public void ClipCursor(ref Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            if (ShouldApplyConstrains)
            {
                Rectangle rect = Mokus2DGame.Instance.ClientBounds;
                rect.Width += rect.X;
                rect.Height += rect.Y;
                ClipCursor(ref rect);
                _applied.SetOn();
            }
            else if (_applied.Use())
            {
                Rectangle rect2 = new(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
                ClipCursor(ref rect2);
            }
        }
    }
}
