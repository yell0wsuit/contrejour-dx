using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRestartIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McRestartIcon";

    public string Id => "menu/McRestartIcon";

    public static McRestartIcon New()
    {
        McRestartIcon mcRestartIcon = StaticPool<McRestartIcon>.New();
        mcRestartIcon.RefreshProperties();
        return mcRestartIcon;
    }

    public McRestartIcon()
        : base("menu/McRestartIcon")
    {
    }

    public void Free()
    {
        StaticPool<McRestartIcon>.Free(this);
    }
}
