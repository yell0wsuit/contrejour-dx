using System;

namespace Mokus2D.Collections.ForEach;

public struct ForEachListUsing : IDisposable
{
	private readonly IForEachList _list;

	public ForEachListUsing(IForEachList list)
	{
		_list = list;
		_list.StartForEach();
	}

	public void Dispose()
	{
		_list.EndForEach();
	}
}
