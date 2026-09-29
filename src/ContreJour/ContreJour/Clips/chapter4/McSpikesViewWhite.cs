using System.CodeDom.Compiler;


using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpikesViewWhite : AnimationNode, IFreeable, IId, ISpikesView
    {
        public const string ID = "chapter4/McSpikesViewWhite";

        public McSpikesPartWhite left { get; protected set; }

        public McSpikesPartWhiteRight right { get; protected set; }

        public McSpikesCenterWhite instance5248497 { get; protected set; }

        public string Id => "chapter4/McSpikesViewWhite";

        MovieClip ISpikesView.Left => left;

        MovieClip ISpikesView.Right => right;

        public static McSpikesViewWhite New()
        {
            McSpikesViewWhite mcSpikesViewWhite = StaticPool.New<McSpikesViewWhite>();
            mcSpikesViewWhite.RefreshProperties();
            return mcSpikesViewWhite;
        }

        public McSpikesViewWhite()
            : base("chapter4/McSpikesViewWhite")
        {
            left = new McSpikesPartWhite();
            AddChild("left", left);
            right = new McSpikesPartWhiteRight();
            AddChild("right", right);
            instance5248497 = new McSpikesCenterWhite();
            AddChild("instance5248497", instance5248497);
            Initialize();
        }

        public void Free()
        {
            StaticPool.Free<McSpikesViewWhite>(this);
        }
    }
}
