using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMenuGroundPhone : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMenuGroundPhone";

    public string Id => "menu/McMenuGroundPhone";

    public static McMenuGroundPhone New()
    {
        McMenuGroundPhone mcMenuGroundPhone = StaticPool<McMenuGroundPhone>.New();
        mcMenuGroundPhone.RefreshProperties();
        return mcMenuGroundPhone;
    }

    public McMenuGroundPhone()
        : base("menu/McMenuGroundPhone")
    {
    }

    public void Free()
    {
        StaticPool<McMenuGroundPhone>.Free(this);
    }
}
