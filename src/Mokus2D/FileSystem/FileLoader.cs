using System;
using System.IO;

namespace Mokus2D.FileSystem
{
    public class FileLoader : IFileLoader
    {
        public Stream OpenFile(string path)
        {
            Stream stream = OpenStream(path) ?? throw new FileNotFoundException(path);
            return stream;
        }

        private static FileStream OpenStream(string name)
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, name.Replace('\\', '/'));
                return File.OpenRead(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
