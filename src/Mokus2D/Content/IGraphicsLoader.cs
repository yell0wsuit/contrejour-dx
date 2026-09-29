using System;

namespace Mokus2D.Content
{
    public interface IGraphicsLoader
    {
        string GraphicsRootDirectory { get; set; }

        bool IsAbsolutePath { get; set; }

        bool FallbackToDefaultScaleFactor { get; set; }

        float PrefferedScaleFactor { get; set; }

        event Action<string, object> ResourceLoaded;

        T Load<T>(string name);
    }
}
