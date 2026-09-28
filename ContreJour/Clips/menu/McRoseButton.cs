using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseButton : Sprite, IFreeable, IId
{
    public const string ID = "menu/McRoseButton";

    public string Id => "menu/McRoseButton";

    public static McRoseButton New()
    {
        McRoseButton mcRoseButton = StaticPool<McRoseButton>.New();
        mcRoseButton.RefreshProperties();
        return mcRoseButton;
    }

    public McRoseButton()
        : base("menu/McRoseButton")
    {
    }

    public void Free()
    {
        StaticPool<McRoseButton>.Free(this);
    }
}
