using System.IO;

using Microsoft.Xna.Framework;

namespace Mokus2D.FileSystem
{
    public class TitleContainerLoader : IFileLoader
    {
        public Stream OpenFile(string path)
        {
            return TitleContainer.OpenStream(path);
        }
    }
}
