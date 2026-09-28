using System.IO;

namespace Mokus2D.FileSystem;

public interface IFileLoader
{
	Stream OpenFile(string path);
}
