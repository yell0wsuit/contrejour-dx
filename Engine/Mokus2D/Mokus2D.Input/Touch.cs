using System;
using Microsoft.Xna.Framework;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Input;

public class Touch
{
	private bool _active = true;

	private int _id;

	private Vector2 _position;

	private Vector2 _previousPosition;

	public bool Stoped { get; private set; }

	public TouchType Type { get; private set; }

	public Vector2 TotalOffset => _position - InitialPosition;

	public Vector2 LastFrameOffset => _position - _previousPosition;

	public Vector2 InitialPosition { get; private set; }

	public Vector2 MaxOffset { get; private set; }

	public DateTime StartTimeUTC { get; private set; }

	public bool Active
	{
		get
		{
			return _active;
		}
		internal set
		{
			_active = value;
		}
	}

	public int Id => _id;

	public Vector2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_previousPosition = _position;
			_position = value;
			MaxOffset = Vector2.Max((Position - InitialPosition).Abs(), MaxOffset);
		}
	}

	public void Initialize(CursorPoint point)
	{
		_id = point.Id;
		InitialPosition = (_previousPosition = (_position = point.Position));
		MaxOffset = Vector2.Zero;
		Type = point.Type;
		StartTimeUTC = DateTime.UtcNow;
	}

	public void StopPropagation()
	{
		Stoped = true;
	}

	public void Refresh()
	{
		Stoped = false;
	}
}
