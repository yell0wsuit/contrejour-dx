using System;
using System.Diagnostics;

namespace Mokus2D.Visual.GameDebug;

public struct GarbageTracer : IDisposable
{
	private const string INFO_MESSAGE_ARG = "Garbage Generation in {0} {1} bytes";

	private readonly string _name;

	private long _memory;

	public GarbageTracer(string name, bool start = true)
	{
		this = default(GarbageTracer);
		name.Intern();
		_name = name;
	}

	public bool Refresh()
	{
		if (!Mokus2DGame.Config.DebugConfig.DebugGarbageGeneration)
		{
			return false;
		}
		long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
		bool result = totalMemory != _memory;
		_memory = totalMemory;
		return result;
	}

	[Conditional("DEBUG")]
	public void Start()
	{
		_memory = GC.GetTotalMemory(forceFullCollection: false);
	}

	[Conditional("DEBUG")]
	public void End()
	{
		if (Mokus2DGame.Config.DebugConfig.DebugGarbageGeneration)
		{
			long num = GC.GetTotalMemory(forceFullCollection: false) - _memory;
			if (num > 0)
			{
				Trace.TraceWarning("Garbage Generation in {0} {1} bytes", _name, num);
			}
		}
	}

	public void Dispose()
	{
	}
}
