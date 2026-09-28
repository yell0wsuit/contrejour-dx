using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEnergyIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McEnergyIcon";

    public string Id => "menu/McEnergyIcon";

    public static McEnergyIcon New()
    {
        McEnergyIcon mcEnergyIcon = StaticPool<McEnergyIcon>.New();
        mcEnergyIcon.RefreshProperties();
        return mcEnergyIcon;
    }

    public McEnergyIcon()
        : base("menu/McEnergyIcon")
    {
    }

    public void Free()
    {
        StaticPool<McEnergyIcon>.Free(this);
    }
}
