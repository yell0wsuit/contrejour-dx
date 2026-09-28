using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSimpleSpikesViewBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common/McSimpleSpikesViewBlack";

    public string Id => "common/McSimpleSpikesViewBlack";

    public static McSimpleSpikesViewBlack New()
    {
        McSimpleSpikesViewBlack mcSimpleSpikesViewBlack = StaticPool<McSimpleSpikesViewBlack>.New();
        mcSimpleSpikesViewBlack.RefreshProperties();
        return mcSimpleSpikesViewBlack;
    }

    public McSimpleSpikesViewBlack()
        : base("common/McSimpleSpikesViewBlack")
    {
    }

    public void Free()
    {
        StaticPool<McSimpleSpikesViewBlack>.Free(this);
    }
}
