using System.CodeDom.Compiler;


using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesView : AnimationNode, IFreeable, IId, ISpikesView
{
    public const string ID = "common/McSpikesView";

    public McSpikesPart left { get; protected set; }

    public McSpikesPart right { get; protected set; }

    public McSpikesCenter instance5241334 { get; protected set; }

    public string Id => "common/McSpikesView";

    MovieClip ISpikesView.Left => left;

    MovieClip ISpikesView.Right => right;

    public static McSpikesView New()
    {
        McSpikesView mcSpikesView = StaticPool.New<McSpikesView>();
        mcSpikesView.RefreshProperties();
        return mcSpikesView;
    }

    public McSpikesView()
        : base("common/McSpikesView")
    {
        left = new McSpikesPart();
        AddChild("left", left);
        right = new McSpikesPart();
        AddChild("right", right);
        instance5241334 = new McSpikesCenter();
        AddChild("instance5241334", instance5241334);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McSpikesView>(this);
    }
}
