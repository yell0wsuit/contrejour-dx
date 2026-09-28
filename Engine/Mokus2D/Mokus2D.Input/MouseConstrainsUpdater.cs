using System;
using Microsoft.Xna.Framework;
using Mokus2D.Util;

namespace Mokus2D.Input;

public class MouseConstrainsUpdater
{
	private readonly Flag _applied = new Flag(on: false);

	protected virtual bool ShouldApplyConstrains
	{
		get
		{
			if (Mokus2DGame.Instance.AcceptsInput)
			{
				return Mokus2DGame.Instance.IsFullScreen;
			}
			return false;
		}
	}

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
			Rectangle rect2 = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
			ClipCursor(ref rect2);
		}
	}
}
