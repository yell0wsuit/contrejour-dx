using System;
using Mokus2D.Interfaces;

namespace Mokus2D.Util.Schedule;

public class TimeoutScheduler : IUpdatable
{
	private float _currentTimeout;

	public event Action TimeoutEvent;

	public void Start(float timeout)
	{
		_currentTimeout = timeout;
	}

	public void Cancel()
	{
		_currentTimeout = 0f;
	}

	public void Update(float time)
	{
		if (_currentTimeout > 0f)
		{
			_currentTimeout -= time;
			if (_currentTimeout <= 0f)
			{
				this.TimeoutEvent.Dispatch();
			}
		}
	}
}
