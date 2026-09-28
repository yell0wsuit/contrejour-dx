using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.spikes;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McCircleSpikesView : MovieClip, IFreeable, IId
{
    public const string ID = "spikes/McCircleSpikesView";

    public string Id => "spikes/McCircleSpikesView";

    public static McCircleSpikesView New()
    {
        McCircleSpikesView mcCircleSpikesView = StaticPool<McCircleSpikesView>.New();
        mcCircleSpikesView.RefreshProperties();
        return mcCircleSpikesView;
    }

    public McCircleSpikesView()
        : base("spikes/McCircleSpikesView")
    {
    }

    public void Free()
    {
        StaticPool<McCircleSpikesView>.Free(this);
    }
}
