using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground3_4Content : Sprite, IFreeable, IId
{
    public const string ID = "chapter3/McBackground3_4Content";

    public string Id => "chapter3/McBackground3_4Content";

    public static McBackground3_4Content New()
    {
        McBackground3_4Content mcBackground3_4Content = StaticPool<McBackground3_4Content>.New();
        mcBackground3_4Content.RefreshProperties();
        return mcBackground3_4Content;
    }

    public McBackground3_4Content()
        : base("chapter3/McBackground3_4Content")
    {
    }

    public void Free()
    {
        StaticPool<McBackground3_4Content>.Free(this);
    }
}
