using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGroundPartBlack : Sprite, IFreeable, IId
{
    public const string ID = "common/McGroundPartBlack";

    public string Id => "common/McGroundPartBlack";

    public static McGroundPartBlack New()
    {
        McGroundPartBlack mcGroundPartBlack = StaticPool<McGroundPartBlack>.New();
        mcGroundPartBlack.RefreshProperties();
        return mcGroundPartBlack;
    }

    public McGroundPartBlack()
        : base("common/McGroundPartBlack")
    {
    }

    public void Free()
    {
        StaticPool<McGroundPartBlack>.Free(this);
    }
}
