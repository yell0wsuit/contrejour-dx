using System;
using System.Threading.Tasks;

using ContreJour.Browser.Platform;

namespace ContreJour.Browser
{
    // Fetches everything the game reads before it is allowed to start, so every later read is synchronous: the
    // metadata bundle and the images and fonts into the file loader. Audio is the audio backend's.
    internal static class ContentPreloader
    {
        private const int Concurrency = 8;

        public static async Task<ContentCatalog> LoadAsync(string baseUrl, BrowserFileLoader files)
        {
            PageInterop.ReportProgress("metadata", 0, 1);
            MetadataBundle.Read(await FetchRequiredAsync(baseUrl + "metadata.json"), files.Add);
            PageInterop.ReportProgress("metadata", 1, 1);
            ContentCatalog catalog = ContentCatalog.Read(await FetchRequiredAsync(baseUrl + "assets.json"));
            await PreloadAsync("images", catalog.Images, baseUrl, files);
            await PreloadAsync("fonts", catalog.Fonts, baseUrl, files);
            return catalog;
        }

        public static Task RunAsync(string label, string[] paths, Func<string, Task> load)
        {
            PageInterop.ReportProgress(label, 0, paths.Length);
            return ParallelPump.RunAsync(paths, Concurrency, load, done => PageInterop.ReportProgress(label, done, paths.Length));
        }

        private static Task PreloadAsync(string label, string[] paths, string baseUrl, BrowserFileLoader files)
        {
            return RunAsync(label, paths, async path => files.Add(path, await FetchRequiredAsync(baseUrl + path)));
        }

        private static async Task<byte[]> FetchRequiredAsync(string url)
        {
            byte[] bytes = await FetchInterop.GetBytesAsync(url);
            return bytes.Length != 0
                ? bytes
                : throw new InvalidOperationException($"Could not load {url}. Run tools/build_web_content.py first.");
        }
    }
}
