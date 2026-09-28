using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground5Content : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McBackground5Content";

    public string Id => "chapter1/McBackground5Content";

    public static McBackground5Content New()
    {
        McBackground5Content mcBackground5Content = StaticPool<McBackground5Content>.New();
        mcBackground5Content.RefreshProperties();
        return mcBackground5Content;
    }

    public McBackground5Content()
        : base("chapter1/McBackground5Content")
    {
    }

    public void Free()
    {
        StaticPool<McBackground5Content>.Free(this);
    }
}
