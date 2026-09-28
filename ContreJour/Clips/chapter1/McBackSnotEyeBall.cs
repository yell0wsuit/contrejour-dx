using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackSnotEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McBackSnotEyeBall";

    public string Id => "chapter1/McBackSnotEyeBall";

    public static McBackSnotEyeBall New()
    {
        McBackSnotEyeBall mcBackSnotEyeBall = StaticPool<McBackSnotEyeBall>.New();
        mcBackSnotEyeBall.RefreshProperties();
        return mcBackSnotEyeBall;
    }

    public McBackSnotEyeBall()
        : base("chapter1/McBackSnotEyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McBackSnotEyeBall>.Free(this);
    }
}
