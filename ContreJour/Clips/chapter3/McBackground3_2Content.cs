using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground3_2Content : Sprite, IFreeable, IId
{
    public const string ID = "chapter3/McBackground3_2Content";

    public string Id => "chapter3/McBackground3_2Content";

    public static McBackground3_2Content New()
    {
        McBackground3_2Content mcBackground3_2Content = StaticPool.New<McBackground3_2Content>();
        mcBackground3_2Content.RefreshProperties();
        return mcBackground3_2Content;
    }

    public McBackground3_2Content()
        : base("chapter3/McBackground3_2Content")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackground3_2Content>(this);
    }
}
