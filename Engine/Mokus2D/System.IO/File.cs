using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace System.IO;

public static class File
{
	public static bool Exists(string fileName)
	{
		return Task.Run(() => FileExistsAsync(fileName).Result).Result;
	}

	private static async Task<bool> FileExistsAsync(string fileName)
	{
		Package package = Package.Current;
		try
		{
			TaskAwaiter<StorageFile> taskAwaiter = WindowsRuntimeSystemExtensions.GetAwaiter<StorageFile>(package.InstalledLocation.GetFileAsync(fileName));
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<StorageFile> taskAwaiter2 = default(TaskAwaiter<StorageFile>);
				taskAwaiter = taskAwaiter2;
			}
			taskAwaiter.GetResult();
			return true;
		}
		catch (FileNotFoundException)
		{
			return false;
		}
	}
}
