using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSoundIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McSoundIcon";

    public string Id => "menu/McSoundIcon";

    public static McSoundIcon New()
    {
        McSoundIcon mcSoundIcon = StaticPool<McSoundIcon>.New();
        mcSoundIcon.RefreshProperties();
        return mcSoundIcon;
    }

    public McSoundIcon()
        : base("menu/McSoundIcon")
    {
    }

    public void Free()
    {
        StaticPool<McSoundIcon>.Free(this);
    }
}
