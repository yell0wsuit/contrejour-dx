using System;
using System.Collections.Generic;

namespace Mokus2D.Util.Schedule;

public class Scheduler : Updater
{
	private struct SchedulerTask(Action action, float timeLeft)
	{
		public readonly Action Action = action;

		public float TimeLeft = timeLeft;
	}

	private readonly List<SchedulerTask> _tasks = new List<SchedulerTask>();

	private readonly List<Action> _toRun = new List<Action>();

	public void Schedule(Action action)
	{
		action();
	}

	public void Schedule(Action action, float seconds)
	{
		SchedulerTask item = new SchedulerTask(action, seconds);
		_tasks.Add(item);
	}

	public void Cancel(Action action)
	{
		_tasks.RemoveAll((SchedulerTask t) => t.Action == action);
	}

	public override void Update(float time)
	{
		if (Paused)
		{
			return;
		}
		base.Update(time);
		for (int num = _tasks.Count - 1; num >= 0; num--)
		{
			SchedulerTask value = _tasks[num];
			value.TimeLeft -= time;
			_tasks[num] = value;
			if (value.TimeLeft <= 0f)
			{
				_toRun.Add(value.Action);
				_tasks.RemoveAt(num);
			}
		}
		foreach (Action item in _toRun)
		{
			item();
		}
		_toRun.Clear();
	}
}
