using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpikesViewWhite : AnimationNode, IId, ISpikesView
    {
        public const string ID = "chapter4/McSpikesViewWhite";

        public MovieClip left { get; protected set; }

        public MovieClip right { get; protected set; }

        public Sprite instance5248497 { get; protected set; }

        public string Id => "chapter4/McSpikesViewWhite";

        MovieClip ISpikesView.Left => left;

        MovieClip ISpikesView.Right => right;

        public McSpikesViewWhite()
            : base("chapter4/McSpikesViewWhite")
        {
            left = new MovieClip(ClipIds.Chapter4.McSpikesPartWhite);
            AddChild("left", left);
            right = new MovieClip(ClipIds.Chapter4.McSpikesPartWhiteRight);
            AddChild("right", right);
            instance5248497 = new Sprite(ClipIds.Chapter4.McSpikesCenterWhite);
            AddChild("instance5248497", instance5248497);
            Initialize();
        }
    }
}
