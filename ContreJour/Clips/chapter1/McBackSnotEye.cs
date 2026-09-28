using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackSnotEye : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McBackSnotEye";

    public string Id => "chapter1/McBackSnotEye";

    public static McBackSnotEye New()
    {
        McBackSnotEye mcBackSnotEye = StaticPool<McBackSnotEye>.New();
        mcBackSnotEye.RefreshProperties();
        return mcBackSnotEye;
    }

    public McBackSnotEye()
        : base("chapter1/McBackSnotEye")
    {
    }

    public void Free()
    {
        StaticPool<McBackSnotEye>.Free(this);
    }
}
