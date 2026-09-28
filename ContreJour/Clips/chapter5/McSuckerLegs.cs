using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerLegs : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSuckerLegs";

    public string Id => "chapter5/McSuckerLegs";

    public static McSuckerLegs New()
    {
        McSuckerLegs mcSuckerLegs = StaticPool<McSuckerLegs>.New();
        mcSuckerLegs.RefreshProperties();
        return mcSuckerLegs;
    }

    public McSuckerLegs()
        : base("chapter5/McSuckerLegs")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerLegs>.Free(this);
    }
}
