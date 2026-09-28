using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevelEnergyInactive : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevelEnergyInactive";

    public string Id => "menu/McLevelEnergyInactive";

    public static McLevelEnergyInactive New()
    {
        McLevelEnergyInactive mcLevelEnergyInactive = StaticPool<McLevelEnergyInactive>.New();
        mcLevelEnergyInactive.RefreshProperties();
        return mcLevelEnergyInactive;
    }

    public McLevelEnergyInactive()
        : base("menu/McLevelEnergyInactive")
    {
    }

    public void Free()
    {
        StaticPool<McLevelEnergyInactive>.Free(this);
    }
}
