using Mokus2D.Game;
using MonoGame.Framework;
using Windows.ApplicationModel.Core;

namespace ContreJour;

public static class Program
{
    private static void Main()
    {
        CoreApplication.Run((IFrameworkViewSource)(object)new GameFrameworkViewSource<Mokus2DApplication<ContreJourApplication>>());
    }
}
