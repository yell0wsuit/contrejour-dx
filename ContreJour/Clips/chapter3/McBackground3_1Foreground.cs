using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground3_1Foreground : Sprite, IFreeable, IId
{
    public const string ID = "chapter3/McBackground3_1Foreground";

    public string Id => "chapter3/McBackground3_1Foreground";

    public static McBackground3_1Foreground New()
    {
        McBackground3_1Foreground mcBackground3_1Foreground = StaticPool<McBackground3_1Foreground>.New();
        mcBackground3_1Foreground.RefreshProperties();
        return mcBackground3_1Foreground;
    }

    public McBackground3_1Foreground()
        : base("chapter3/McBackground3_1Foreground")
    {
    }

    public void Free()
    {
        StaticPool<McBackground3_1Foreground>.Free(this);
    }
}
