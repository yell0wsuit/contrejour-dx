using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Mokus2D.FileSystem;

public class FileLoader : IFileLoader
{
	private const char InvalidPathSpearator = '/';

	private const char PathSpearator = '\\';

	public Stream OpenFile(string path)
	{
		Stream stream = OpenStream(path);
		if (stream == null)
		{
			throw new FileNotFoundException(path);
		}
		return stream;
	}

	private static Stream OpenStream(string name)
	{
		try
		{
			string text = Path.Combine(new string[2]
			{
				Package.Current.InstalledLocation.Path,
				name.Replace('/', '\\')
			});
			StorageFile result = WindowsRuntimeSystemExtensions.GetAwaiter<StorageFile>(StorageFile.GetFileFromPathAsync(text)).GetResult();
			IRandomAccessStreamWithContentType result2 = WindowsRuntimeSystemExtensions.GetAwaiter<IRandomAccessStreamWithContentType>(result.OpenReadAsync()).GetResult();
			return WindowsRuntimeStreamExtensions.AsStreamForRead((IInputStream)(object)result2);
		}
		catch (IOException)
		{
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
