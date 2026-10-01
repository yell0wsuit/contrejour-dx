using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRotatorGrassAll : AnimationNode, IId
    {
        public const string ID = "chapter5/McRotatorGrassAll";

        public Sprite instance5244712 { get; protected set; }

        public string Id => "chapter5/McRotatorGrassAll";

        public McRotatorGrassAll()
            : base("chapter5/McRotatorGrassAll")
        {
            instance5244712 = new Sprite(ClipIds.Chapter5.McRotatorGrass0);
            AddChild("instance5244712", instance5244712);
            Initialize();
        }
    }
}
