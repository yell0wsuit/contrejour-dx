using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBaloonLegs : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McBaloonLegs";

    public string Id => "chapter5/McBaloonLegs";

    public static McBaloonLegs New()
    {
        McBaloonLegs mcBaloonLegs = StaticPool<McBaloonLegs>.New();
        mcBaloonLegs.RefreshProperties();
        return mcBaloonLegs;
    }

    public McBaloonLegs()
        : base("chapter5/McBaloonLegs")
    {
    }

    public void Free()
    {
        StaticPool<McBaloonLegs>.Free(this);
    }
}
