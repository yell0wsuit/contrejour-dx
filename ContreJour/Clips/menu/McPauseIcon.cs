using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPauseIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McPauseIcon";

    public string Id => "menu/McPauseIcon";

    public static McPauseIcon New()
    {
        McPauseIcon mcPauseIcon = StaticPool<McPauseIcon>.New();
        mcPauseIcon.RefreshProperties();
        return mcPauseIcon;
    }

    public McPauseIcon()
        : base("menu/McPauseIcon")
    {
    }

    public void Free()
    {
        StaticPool<McPauseIcon>.Free(this);
    }
}
