using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSimpleSpikesViewWhite : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McSimpleSpikesViewWhite";

    public string Id => "chapter4/McSimpleSpikesViewWhite";

    public static McSimpleSpikesViewWhite New()
    {
        McSimpleSpikesViewWhite mcSimpleSpikesViewWhite = StaticPool<McSimpleSpikesViewWhite>.New();
        mcSimpleSpikesViewWhite.RefreshProperties();
        return mcSimpleSpikesViewWhite;
    }

    public McSimpleSpikesViewWhite()
        : base("chapter4/McSimpleSpikesViewWhite")
    {
    }

    public void Free()
    {
        StaticPool<McSimpleSpikesViewWhite>.Free(this);
    }
}
