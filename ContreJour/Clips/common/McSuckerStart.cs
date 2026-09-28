using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerStart : Sprite, IFreeable, IId
{
    public const string ID = "common/McSuckerStart";

    public string Id => "common/McSuckerStart";

    public static McSuckerStart New()
    {
        McSuckerStart mcSuckerStart = StaticPool<McSuckerStart>.New();
        mcSuckerStart.RefreshProperties();
        return mcSuckerStart;
    }

    public McSuckerStart()
        : base("common/McSuckerStart")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerStart>.Free(this);
    }
}
