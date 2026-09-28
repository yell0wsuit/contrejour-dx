using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSkipIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McSkipIcon";

    public string Id => "menu/McSkipIcon";

    public static McSkipIcon New()
    {
        McSkipIcon mcSkipIcon = StaticPool<McSkipIcon>.New();
        mcSkipIcon.RefreshProperties();
        return mcSkipIcon;
    }

    public McSkipIcon()
        : base("menu/McSkipIcon")
    {
    }

    public void Free()
    {
        StaticPool<McSkipIcon>.Free(this);
    }
}
