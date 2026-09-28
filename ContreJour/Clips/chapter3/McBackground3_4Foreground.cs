using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground3_4Foreground : Sprite, IFreeable, IId
{
    public const string ID = "chapter3/McBackground3_4Foreground";

    public string Id => "chapter3/McBackground3_4Foreground";

    public static McBackground3_4Foreground New()
    {
        McBackground3_4Foreground mcBackground3_4Foreground = StaticPool<McBackground3_4Foreground>.New();
        mcBackground3_4Foreground.RefreshProperties();
        return mcBackground3_4Foreground;
    }

    public McBackground3_4Foreground()
        : base("chapter3/McBackground3_4Foreground")
    {
    }

    public void Free()
    {
        StaticPool<McBackground3_4Foreground>.Free(this);
    }
}
