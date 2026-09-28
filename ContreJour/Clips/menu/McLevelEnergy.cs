using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevelEnergy : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevelEnergy";

    public string Id => "menu/McLevelEnergy";

    public static McLevelEnergy New()
    {
        McLevelEnergy mcLevelEnergy = StaticPool<McLevelEnergy>.New();
        mcLevelEnergy.RefreshProperties();
        return mcLevelEnergy;
    }

    public McLevelEnergy()
        : base("menu/McLevelEnergy")
    {
    }

    public void Free()
    {
        StaticPool<McLevelEnergy>.Free(this);
    }
}
