using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Mokus2D.Input;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Windows.Devices.Input;

namespace Mokus2D.Platforms.Input;

public static class CursorPointsFiller
{
	private const int LeftMouseButtonId = -1;

	private const int RightMouseButtonId = -2;

	private const int MiddleMouseButtonId = -3;

	private const float MinMouseStepLength = 2f;

	private static Vector2? _mousePosition;

	private static readonly MouseCapabilities MouseCapabilities = new MouseCapabilities();

	public static void FillPoints(List<CursorPoint> cursorPoints)
	{
		MouseState state = Mouse.GetState();
		Vector2 mousePosition = state.Position();
		mousePosition = AdjustMouseSpeed(mousePosition);
		bool flag = false;
		foreach (TouchLocation item in TouchPanel.GetState())
		{
			if (item.State == TouchLocationState.Pressed || item.State == TouchLocationState.Moved)
			{
				cursorPoints.Add(new CursorPoint(item.Position, item.Id, TouchType.Touch));
				if (item.Position == new Vector2(state.X, state.Y))
				{
					flag = true;
				}
			}
		}
		bool mouseButtonsSwapped = GetMouseButtonsSwapped();
		ButtonState buttonState = (mouseButtonsSwapped ? state.RightButton : state.LeftButton);
		ButtonState buttonState2 = (mouseButtonsSwapped ? state.LeftButton : state.RightButton);
		if (buttonState == ButtonState.Pressed && !flag)
		{
			cursorPoints.Add(new CursorPoint(mousePosition, -1, TouchType.LeftMouseButton));
		}
		if (buttonState2 == ButtonState.Pressed)
		{
			cursorPoints.Add(new CursorPoint(mousePosition, -2, TouchType.RightMouseButton));
		}
		if (state.MiddleButton == ButtonState.Pressed)
		{
			cursorPoints.Add(new CursorPoint(mousePosition, -3, TouchType.MiddleMouseButton));
		}
	}

	private static Vector2 AdjustMouseSpeed(Vector2 mousePosition)
	{
		if (Mokus2DGame.Config.MouseSpeed.FuzzyEquals(1f, 0.05f))
		{
			return mousePosition;
		}
		if (_mousePosition.HasValue && Mokus2DGame.Instance.IsFullScreen)
		{
			Vector2 vector = mousePosition - _mousePosition.Value;
			if (vector.Length() > 2f)
			{
				vector *= Mokus2DGame.Config.MouseSpeed;
				mousePosition = _mousePosition.Value + vector;
				Mouse.SetPosition((int)mousePosition.X, (int)mousePosition.Y);
			}
		}
		_mousePosition = mousePosition;
		return mousePosition;
	}

	private static bool GetMouseButtonsSwapped()
	{
		return MouseCapabilities.SwapButtons.ToBool();
	}
}
