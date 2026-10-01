using System;
using System.Runtime.Versioning;

using ContreJour.Browser;
using ContreJour.Browser.Platform;
using ContreJour.Saving;

using Mokus2D.Localization;
using Mokus2D.Sound;

[assembly: SupportedOSPlatform("browser")]

await PageInterop.ImportAsync();
await FetchInterop.ImportAsync();
await StorageInterop.ImportAsync();
await HostEventInterop.ImportAsync();
await AudioInterop.ImportAsync();

// Before anything reads it: Messages and ContreJourLabelUtil take the locale in static initializers.
string language = BrowserLanguage.Parse(PageInterop.Query(), PageInterop.NavigatorLanguage());
LocalizationBundle.LocaleOverride = language;

int[] canvas = PageInterop.MeasureCanvas("game");
if (canvas.Length != 4 || HostShim.AcquireCanvas() == 0)
{
    throw new InvalidOperationException("The page has no canvas to draw to.");
}
if (HostShim.CreateContext(canvas[2], canvas[3]) == 0)
{
    throw new InvalidOperationException("Could not create a WebGL2 context.");
}
SkiaSurface surface = new(0, canvas[2], canvas[3]);

const string ContentUrl = "./content/";
BrowserFileLoader files = new();
ContentCatalog catalog = await ContentPreloader.LoadAsync(ContentUrl, files);
IAudioBackend audio;
if (catalog.Sounds.Length + catalog.Songs.Length == 0)
{
    // A payload built with --skip-audio: the game runs silent rather than failing at its first sound.
    Console.WriteLine("cj-audio: none in the content; running silent");
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
Console.WriteLine($"cj-boot-complete: language {language}, {catalog.Images.Length} images, {catalog.Fonts.Length} fonts, {catalog.Sounds.Length} sounds, {catalog.Songs.Length} songs");
