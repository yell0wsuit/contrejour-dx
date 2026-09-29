using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevelItemInactive : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevelItemInactive";

    public string Id => "menu/McLevelItemInactive";

    public static McLevelItemInactive New()
    {
        McLevelItemInactive mcLevelItemInactive = StaticPool.New<McLevelItemInactive>();
        mcLevelItemInactive.RefreshProperties();
        return mcLevelItemInactive;
    }

    public McLevelItemInactive()
        : base("menu/McLevelItemInactive")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLevelItemInactive>(this);
    }
}
