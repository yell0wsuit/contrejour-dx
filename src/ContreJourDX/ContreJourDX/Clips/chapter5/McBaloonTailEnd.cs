using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBaloonTailEnd : AnimationNode, IId
    {
        public const string ID = "chapter5/McBaloonTailEnd";

        public Sprite instance5244474 { get; protected set; }

        public string Id => "chapter5/McBaloonTailEnd";

        public McBaloonTailEnd()
            : base("chapter5/McBaloonTailEnd")
        {
            instance5244474 = new Sprite(ClipIds.Chapter5.McRotatorGrass0);
            AddChild("instance5244474", instance5244474);
            Initialize();
        }
    }
}
