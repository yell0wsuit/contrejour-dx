using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSimpleSpikesView_5 : MovieClip, IFreeable, IId
{
    public const string ID = "chapter5/McSimpleSpikesView_5";

    public string Id => "chapter5/McSimpleSpikesView_5";

    public static McSimpleSpikesView_5 New()
    {
        McSimpleSpikesView_5 mcSimpleSpikesView_ = StaticPool.New<McSimpleSpikesView_5>();
        mcSimpleSpikesView_.RefreshProperties();
        return mcSimpleSpikesView_;
    }

    public McSimpleSpikesView_5()
        : base("chapter5/McSimpleSpikesView_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSimpleSpikesView_5>(this);
    }
}
