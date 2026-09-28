using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerBody : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSuckerBody";

    public string Id => "chapter5/McSuckerBody";

    public static McSuckerBody New()
    {
        McSuckerBody mcSuckerBody = StaticPool<McSuckerBody>.New();
        mcSuckerBody.RefreshProperties();
        return mcSuckerBody;
    }

    public McSuckerBody()
        : base("chapter5/McSuckerBody")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerBody>.Free(this);
    }
}
