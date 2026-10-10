using System;
using System.Collections.Generic;
using System.IO;

using Mokus2D.FileSystem;

namespace ContreJourDX.Browser.Platform
{
    // Serves content preloaded before the game starts. Lookups ignore case and slash direction: the game was
    // written against case-insensitive file systems, and it builds paths with Path.Combine.
    public sealed class BrowserFileLoader : IFileLoader
    {
        private readonly Dictionary<string, byte[]> _files = new(StringComparer.OrdinalIgnoreCase);

        public static string Normalize(string path)
        {
            ArgumentNullException.ThrowIfNull(path);
            string normalized = path.Replace('\\', '/');
            while (normalized.StartsWith("./", StringComparison.Ordinal))
            {
                normalized = normalized[2..];
            }
            return normalized.TrimStart('/');
        }

        public void Add(string path, byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            _files[Normalize(path)] = bytes;
        }

        public bool Contains(string path)
        {
            return _files.ContainsKey(Normalize(path));
        }

        // ResourcesLoaderBase falls back between scale factors only on FileNotFoundException.
        public Stream OpenFile(string path)
        {
            return _files.TryGetValue(Normalize(path), out byte[] bytes)
                ? new MemoryStream(bytes, writable: false)
                : throw new FileNotFoundException($"'{path}' is not in the preloaded content.", path);
        }
    }
}
