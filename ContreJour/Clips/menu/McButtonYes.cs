using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonYes : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonYes";

    public string Id => "menu/McButtonYes";

    public static McButtonYes New()
    {
        McButtonYes mcButtonYes = StaticPool<McButtonYes>.New();
        mcButtonYes.RefreshProperties();
        return mcButtonYes;
    }

    public McButtonYes()
        : base("menu/McButtonYes")
    {
    }

    public void Free()
    {
        StaticPool<McButtonYes>.Free(this);
    }
}
