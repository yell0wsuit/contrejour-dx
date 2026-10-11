using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace ContreJourDX.Browser
{
    internal static partial class PageInterop
    {
        public static Task ImportAsync()
        {
            return JSHost.ImportAsync("page", "../page.js");
        }

        // [cssWidth, cssHeight, backingWidth, backingHeight], or empty when the page has no such canvas.
        [JSImport("measureCanvas", "page")]
        public static partial int[] MeasureCanvas(string canvasId);

        // Transfers the canvas to the worker that owns the game and returns its measured size.
        [JSImport("transferCanvasToThread", "page")]
        public static partial int[] TransferCanvasToThread(string canvasId, int threadId);

        // Starts reporting the canvas box and pixel ratio through the event ring.
        [JSImport("watchCanvas", "page")]
        public static partial void WatchCanvas(string canvasId);

        // [width, height, devicePixelRatio]
        [JSImport("screenSize", "page")]
        public static partial double[] ScreenSize();

        [JSImport("query", "page")]
        public static partial string Query();

        [JSImport("navigatorLanguage", "page")]
        public static partial string NavigatorLanguage();

        [JSImport("setLoadingProgress", "page")]
        public static partial void ReportProgress(string type, int loaded, int total);

        [JSImport("setCursorVisible", "page")]
        public static partial void SetCursorVisible(bool visible);
    }
}
