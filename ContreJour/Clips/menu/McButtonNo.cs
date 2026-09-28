using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonNo : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonNo";

    public string Id => "menu/McButtonNo";

    public static McButtonNo New()
    {
        McButtonNo mcButtonNo = StaticPool<McButtonNo>.New();
        mcButtonNo.RefreshProperties();
        return mcButtonNo;
    }

    public McButtonNo()
        : base("menu/McButtonNo")
    {
    }

    public void Free()
    {
        StaticPool<McButtonNo>.Free(this);
    }
}
