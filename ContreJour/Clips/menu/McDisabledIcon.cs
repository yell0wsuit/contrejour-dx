using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McDisabledIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McDisabledIcon";

    public string Id => "menu/McDisabledIcon";

    public static McDisabledIcon New()
    {
        McDisabledIcon mcDisabledIcon = StaticPool.New<McDisabledIcon>();
        mcDisabledIcon.RefreshProperties();
        return mcDisabledIcon;
    }

    public McDisabledIcon()
        : base("menu/McDisabledIcon")
    {
    }

    public void Free()
    {
        StaticPool.Free<McDisabledIcon>(this);
    }
}
