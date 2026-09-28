using System;

namespace Mokus2D.Loop;

public class TicksCalculator
{
	private DateTime _start;

	private float _elapsedSeconds;

	public void Start()
	{
		_start = DateTime.UtcNow;
	}

	public float Update()
	{
		float elapsedSeconds = _elapsedSeconds;
		_elapsedSeconds = (float)(DateTime.UtcNow - _start).TotalSeconds;
		return _elapsedSeconds - elapsedSeconds;
	}
}
