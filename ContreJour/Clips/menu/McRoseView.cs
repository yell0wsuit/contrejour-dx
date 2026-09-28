using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseView : Sprite, IFreeable, IId
{
    public const string ID = "menu/McRoseView";

    public string Id => "menu/McRoseView";

    public static McRoseView New()
    {
        McRoseView mcRoseView = StaticPool<McRoseView>.New();
        mcRoseView.RefreshProperties();
        return mcRoseView;
    }

    public McRoseView()
        : base("menu/McRoseView")
    {
    }

    public void Free()
    {
        StaticPool<McRoseView>.Free(this);
    }
}
