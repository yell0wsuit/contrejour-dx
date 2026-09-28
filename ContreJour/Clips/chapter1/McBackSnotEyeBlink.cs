using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackSnotEyeBlink : MovieClip, IFreeable, IId
{
    public const string ID = "chapter1/McBackSnotEyeBlink";

    public string Id => "chapter1/McBackSnotEyeBlink";

    public static McBackSnotEyeBlink New()
    {
        McBackSnotEyeBlink mcBackSnotEyeBlink = StaticPool<McBackSnotEyeBlink>.New();
        mcBackSnotEyeBlink.RefreshProperties();
        return mcBackSnotEyeBlink;
    }

    public McBackSnotEyeBlink()
        : base("chapter1/McBackSnotEyeBlink")
    {
    }

    public void Free()
    {
        StaticPool<McBackSnotEyeBlink>.Free(this);
    }
}
