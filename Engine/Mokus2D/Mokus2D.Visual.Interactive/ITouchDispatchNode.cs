using System;

namespace Mokus2D.Visual.Interactive;

public interface ITouchDispatchNode
{
	event Action<TouchArguments> TouchBeginEvent;

	event Action<TouchArguments> TouchEndEvent;

	event Action<TouchArguments> TouchOutEvent;

	event Action<TouchArguments> TouchMoveEvent;
}
