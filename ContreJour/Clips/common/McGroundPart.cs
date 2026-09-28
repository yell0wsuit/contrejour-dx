using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGroundPart : Sprite, IFreeable, IId
{
    public const string ID = "common/McGroundPart";

    public string Id => "common/McGroundPart";

    public static McGroundPart New()
    {
        McGroundPart mcGroundPart = StaticPool<McGroundPart>.New();
        mcGroundPart.RefreshProperties();
        return mcGroundPart;
    }

    public McGroundPart()
        : base("common/McGroundPart")
    {
    }

    public void Free()
    {
        StaticPool<McGroundPart>.Free(this);
    }
}
