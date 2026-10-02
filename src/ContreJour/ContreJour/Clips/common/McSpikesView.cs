using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpikesView : AnimationNode, IId, ISpikesView
    {
        public const string ID = "common/McSpikesView";

        public MovieClip left { get; protected set; }

        public MovieClip right { get; protected set; }

        public Sprite instance5241334 { get; protected set; }

        public string Id => "common/McSpikesView";

        MovieClip ISpikesView.Left => left;

        MovieClip ISpikesView.Right => right;

        public McSpikesView()
            : base("common/McSpikesView")
        {
            left = new MovieClip(ClipIds.Common.McSpikesPart);
            AddChild("left", left);
            right = new MovieClip(ClipIds.Common.McSpikesPart);
            AddChild("right", right);
            instance5241334 = new Sprite(ClipIds.Common.McSpikesCenter);
            AddChild("instance5241334", instance5241334);
            Initialize();
        }
    }
}
