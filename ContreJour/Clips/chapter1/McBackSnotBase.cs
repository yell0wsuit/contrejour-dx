using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackSnotBase : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McBackSnotBase";

    public string Id => "chapter1/McBackSnotBase";

    public static McBackSnotBase New()
    {
        McBackSnotBase mcBackSnotBase = StaticPool<McBackSnotBase>.New();
        mcBackSnotBase.RefreshProperties();
        return mcBackSnotBase;
    }

    public McBackSnotBase()
        : base("chapter1/McBackSnotBase")
    {
    }

    public void Free()
    {
        StaticPool<McBackSnotBase>.Free(this);
    }
}
