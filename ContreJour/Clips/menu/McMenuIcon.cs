using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMenuIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMenuIcon";

    public string Id => "menu/McMenuIcon";

    public static McMenuIcon New()
    {
        McMenuIcon mcMenuIcon = StaticPool<McMenuIcon>.New();
        mcMenuIcon.RefreshProperties();
        return mcMenuIcon;
    }

    public McMenuIcon()
        : base("menu/McMenuIcon")
    {
    }

    public void Free()
    {
        StaticPool<McMenuIcon>.Free(this);
    }
}
