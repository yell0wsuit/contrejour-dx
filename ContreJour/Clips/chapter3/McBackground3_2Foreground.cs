using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground3_2Foreground : Sprite, IFreeable, IId
{
    public const string ID = "chapter3/McBackground3_2Foreground";

    public string Id => "chapter3/McBackground3_2Foreground";

    public static McBackground3_2Foreground New()
    {
        McBackground3_2Foreground mcBackground3_2Foreground = StaticPool.New<McBackground3_2Foreground>();
        mcBackground3_2Foreground.RefreshProperties();
        return mcBackground3_2Foreground;
    }

    public McBackground3_2Foreground()
        : base("chapter3/McBackground3_2Foreground")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackground3_2Foreground>(this);
    }
}
