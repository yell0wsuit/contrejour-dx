using System;
using Mokus2D.Util.Resources;

namespace Mokus2D.Util.Schedule;

public class Disposable : DisposableBase
{
	private Action action;

	public Disposable(Action action)
	{
		this.action = action;
	}

	protected override void Dispose(bool disposing)
	{
		if (action != null)
		{
			action();
		}
	}
}
