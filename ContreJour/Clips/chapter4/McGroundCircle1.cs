using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGroundCircle1 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McGroundCircle1";

    public string Id => "chapter4/McGroundCircle1";

    public static McGroundCircle1 New()
    {
        McGroundCircle1 mcGroundCircle = StaticPool<McGroundCircle1>.New();
        mcGroundCircle.RefreshProperties();
        return mcGroundCircle;
    }

    public McGroundCircle1()
        : base("chapter4/McGroundCircle1")
    {
    }

    public void Free()
    {
        StaticPool<McGroundCircle1>.Free(this);
    }
}
