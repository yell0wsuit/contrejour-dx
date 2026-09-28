using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMenuCircleSpikes : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMenuCircleSpikes";

    public string Id => "menu/McMenuCircleSpikes";

    public static McMenuCircleSpikes New()
    {
        McMenuCircleSpikes mcMenuCircleSpikes = StaticPool.New<McMenuCircleSpikes>();
        mcMenuCircleSpikes.RefreshProperties();
        return mcMenuCircleSpikes;
    }

    public McMenuCircleSpikes()
        : base("menu/McMenuCircleSpikes")
    {
    }

    public void Free()
    {
        StaticPool.Free<McMenuCircleSpikes>(this);
    }
}
