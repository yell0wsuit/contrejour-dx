using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace ContreJour.Browser
{
    // fetch.js: bytes over HTTP for the content preload.
    internal static partial class FetchInterop
    {
        public static Task ImportAsync()
        {
            return JSHost.ImportAsync("fetch", "../fetch.js");
        }

        // Fetches a URL into the JS-side stash, returning its length or -1 on failure.
        [JSImport("fetchBytes", "fetch")]
        public static partial Task<int> FetchBytes(string url);

        // Copies the stashed bytes for a URL into a managed buffer, releasing the stash.
        [JSImport("takeStashed", "fetch")]
        public static partial int TakeStashed(string url, [JSMarshalAs<JSType.MemoryView>] Span<byte> destination);

        // The bytes at a URL, or an empty array when the request failed or the body was empty: no shipped
        // file is empty.
        public static async Task<byte[]> GetBytesAsync(string url)
        {
            int length = await FetchBytes(url);
            if (length <= 0)
            {
                return [];
            }
            byte[] buffer = new byte[length];
            _ = TakeStashed(url, buffer);
            return buffer;
        }
    }
}
