using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McCircleSpikesViewWhite : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McCircleSpikesViewWhite";

    public string Id => "chapter4/McCircleSpikesViewWhite";

    public static McCircleSpikesViewWhite New()
    {
        McCircleSpikesViewWhite mcCircleSpikesViewWhite = StaticPool<McCircleSpikesViewWhite>.New();
        mcCircleSpikesViewWhite.RefreshProperties();
        return mcCircleSpikesViewWhite;
    }

    public McCircleSpikesViewWhite()
        : base("chapter4/McCircleSpikesViewWhite")
    {
    }

    public void Free()
    {
        StaticPool<McCircleSpikesViewWhite>.Free(this);
    }
}
