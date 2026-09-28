using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevelComplete : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevelComplete";

    public string Id => "menu/McLevelComplete";

    public static McLevelComplete New()
    {
        McLevelComplete mcLevelComplete = StaticPool<McLevelComplete>.New();
        mcLevelComplete.RefreshProperties();
        return mcLevelComplete;
    }

    public McLevelComplete()
        : base("menu/McLevelComplete")
    {
    }

    public void Free()
    {
        StaticPool<McLevelComplete>.Free(this);
    }
}
