using System;
using System.Runtime.Versioning;
#if WASM_THREADS
using System.Threading.Tasks;
#endif

using ContreJourDX.Browser;
using ContreJourDX.Browser.Platform;
using ContreJourDX.Saving;

using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;
using Mokus2D.Localization;
using Mokus2D.Sound;

[assembly: SupportedOSPlatform("browser")]

// Install before content, audio, or saves can report a failure. The factory stays alive
// for the animation callbacks after this entry point returns.
await LogInterop.ImportAsync();
_ = LogInterop.Begin("Contre Jour DX\nBrowser build\nVersion: " + typeof(BrowserGame).Assembly.GetName().Version);
Log.Factory = new BrowserLogFactory(LogInterop.Append, Console.Out, Console.Error);

await PageInterop.ImportAsync();
await FetchInterop.ImportAsync();
await StorageInterop.ImportAsync();
await HostEventInterop.ImportAsync();
await AudioInterop.ImportAsync();

// Before anything reads it: Messages and ContreJourDXLabelUtil take the locale in static initializers.
string language = BrowserLanguage.Parse(PageInterop.Query(), PageInterop.NavigatorLanguage());
LocalizationBundle.LocaleOverride = language;

// Skia's native GL calls run on their caller, so the game thread must own the
// canvas and context before any Skia surface exists.
#if WASM_THREADS
if (HostShim.IsMainRuntimeThread() != 0)
{
    throw new InvalidOperationException("The threaded game must run on a worker.");
}
if (HostShim.AcquireCanvas() == 0)
{
    throw new InvalidOperationException("Could not prepare the game thread for its canvas.");
}
int[] canvas = PageInterop.TransferCanvasToThread("game", HostShim.ThreadId());
if (canvas.Length != 4)
{
    throw new InvalidOperationException("Could not transfer the canvas to the game thread.");
}

int deliveryAttempts = 0;
while (HostShim.CanvasReceived() == 0 && deliveryAttempts < 200)
{
    deliveryAttempts++;
    await Task.Delay(25);
}
if (HostShim.CanvasReceived() == 0)
{
    throw new InvalidOperationException("The transferred canvas never arrived.");
}
#else
int[] canvas = PageInterop.MeasureCanvas("game");
if (canvas.Length != 4 || HostShim.AcquireCanvas() == 0)
{
    throw new InvalidOperationException("The page has no canvas to draw to.");
}
#endif
if (HostShim.CreateContext(canvas[2], canvas[3]) == 0)
{
    throw new InvalidOperationException("Could not create the game thread's WebGL2 context.");
}
SkiaSurface surface = new(0, canvas[2], canvas[3]);

const string ContentUrl = "./content/";
BrowserFileLoader files = new();
ContentCatalog catalog = await ContentPreloader.LoadAsync(ContentUrl, files);
IAudioBackend audio;
if (catalog.Sounds.Length + catalog.Songs.Length == 0)
{
    // A payload built with --skip-audio: the game runs silent rather than failing at its first sound.
    ILogger noAudioLogger = Log.For(LogCategories.Audio);
    BrowserLog.NoAudio(noAudioLogger);
    audio = new NullAudioBackend();
}
else
{
    WebAudioBackend web = new();
    await web.PreloadAsync(ContentUrl, catalog);
    audio = web;
}

Preferences.Store = new LocalStoragePreferenceStore();
GameLoop.Surface = surface;
GameLoop.CreateGame = (cssSize, pixelSize, timestampMs) => new BrowserGame(surface, files, audio, cssSize, pixelSize, timestampMs);
HostEventQueue.Initialize();
PageInterop.WatchCanvas("game");
GameLoop.Start(canvas[0], canvas[1], canvas[2], canvas[3]);
ILogger bootCompleteLogger = Log.For(LogCategories.Host);
BrowserLog.BootComplete(bootCompleteLogger, language, catalog.Images.Length, catalog.Fonts.Length, catalog.Sounds.Length, catalog.Songs.Length);
