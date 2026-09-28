using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoundDragViewWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McRoundDragViewWhite";

    public string Id => "chapter4/McRoundDragViewWhite";

    public static McRoundDragViewWhite New()
    {
        McRoundDragViewWhite mcRoundDragViewWhite = StaticPool<McRoundDragViewWhite>.New();
        mcRoundDragViewWhite.RefreshProperties();
        return mcRoundDragViewWhite;
    }

    public McRoundDragViewWhite()
        : base("chapter4/McRoundDragViewWhite")
    {
    }

    public void Free()
    {
        StaticPool<McRoundDragViewWhite>.Free(this);
    }
}
