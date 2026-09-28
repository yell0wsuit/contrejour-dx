using System;
using Windows.Foundation;

namespace Mokus2D.Util.Extensions;

public static class AsyncExtensions
{
	public static T AwaitResult<T>(this IAsyncOperation<T> source)
	{
		return WindowsRuntimeSystemExtensions.GetAwaiter<T>(source).GetResult();
	}

	public static void AwaitResult(this IAsyncAction source)
	{
		WindowsRuntimeSystemExtensions.GetAwaiter(source).GetResult();
	}
}
