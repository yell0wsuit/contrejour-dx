using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGroundPartWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McGroundPartWhite";

    public string Id => "chapter4/McGroundPartWhite";

    public static McGroundPartWhite New()
    {
        McGroundPartWhite mcGroundPartWhite = StaticPool.New<McGroundPartWhite>();
        mcGroundPartWhite.RefreshProperties();
        return mcGroundPartWhite;
    }

    public McGroundPartWhite()
        : base("chapter4/McGroundPartWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McGroundPartWhite>(this);
    }
}
