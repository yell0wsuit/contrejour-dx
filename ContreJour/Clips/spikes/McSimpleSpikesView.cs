using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.spikes;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSimpleSpikesView : MovieClip, IFreeable, IId
{
    public const string ID = "spikes/McSimpleSpikesView";

    public string Id => "spikes/McSimpleSpikesView";

    public static McSimpleSpikesView New()
    {
        McSimpleSpikesView mcSimpleSpikesView = StaticPool<McSimpleSpikesView>.New();
        mcSimpleSpikesView.RefreshProperties();
        return mcSimpleSpikesView;
    }

    public McSimpleSpikesView()
        : base("spikes/McSimpleSpikesView")
    {
    }

    public void Free()
    {
        StaticPool<McSimpleSpikesView>.Free(this);
    }
}
