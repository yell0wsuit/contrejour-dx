using System;

namespace Mokus2D.Util.Schedule;

public class FixedTimeActionUpdater : Updater
{
	private readonly float _stepTime;

	private float _currentTime;

	private readonly Action<float> _updateAction;

	public FixedTimeActionUpdater(float stepTime, Action<float> updateAction = null)
	{
		_stepTime = stepTime;
		_updateAction = updateAction;
	}

	public override void Update(float time)
	{
		_currentTime += time;
		while (_currentTime >= _stepTime)
		{
			base.Update(_stepTime);
			if (_updateAction != null)
			{
				_updateAction(_stepTime);
			}
			_currentTime -= _stepTime;
		}
	}

	public void Reset()
	{
		_currentTime = 0f;
	}
}
