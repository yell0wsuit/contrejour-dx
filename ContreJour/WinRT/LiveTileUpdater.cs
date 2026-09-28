using ContreJour.Clips.liveTile;
using Microsoft.Xna.Framework;
using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;

namespace ContreJour.WinRT;

public static class LiveTileUpdater
{
    private const string PathPrefix = "ms-appdata:///local/";

    private static readonly LiveTileDrawer WideDrawer = new LiveTileDrawer(new Vector2(310f, 150f), "wide.png", new liveTileWide());

    private static readonly LiveTileDrawer LargeDrawer = new LiveTileDrawer(new Vector2(310f), "large.png", new liveTileSquare());

    public static async void UpdateTiles(int stars)
    {
        await WideDrawer.DrawLiveTile(stars);
        await LargeDrawer.DrawLiveTile(stars);
        ITileWide310x150Image wide = TileContentFactory.CreateTileWide310x150Image();
        wide.Image.put_Src("ms-appdata:///local/" + WideDrawer.FilePath);
        ((IWide310x150TileNotificationContent)wide).put_RequireSquare150x150Content(false);
        ((ITileNotificationContent)wide).put_Branding((TileBranding)0);
        ITileSquare310x310Image square = TileContentFactory.CreateTileSquare310x310Image();
        square.Image.put_Src("ms-appdata:///local/" + LargeDrawer.FilePath);
        ((ISquare310x310TileNotificationContent)square).put_Wide310x150Content((IWide310x150TileNotificationContent)(object)wide);
        ((ITileNotificationContent)square).put_Branding((TileBranding)0);
        TileNotification notification = ((ITileNotificationContent)square).CreateNotification();
        TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
        updater.Clear();
        updater.Update(notification);
    }
}
