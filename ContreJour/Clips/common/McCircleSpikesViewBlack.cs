using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McCircleSpikesViewBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common/McCircleSpikesViewBlack";

    public string Id => "common/McCircleSpikesViewBlack";

    public static McCircleSpikesViewBlack New()
    {
        McCircleSpikesViewBlack mcCircleSpikesViewBlack = StaticPool<McCircleSpikesViewBlack>.New();
        mcCircleSpikesViewBlack.RefreshProperties();
        return mcCircleSpikesViewBlack;
    }

    public McCircleSpikesViewBlack()
        : base("common/McCircleSpikesViewBlack")
    {
    }

    public void Free()
    {
        StaticPool<McCircleSpikesViewBlack>.Free(this);
    }
}
