using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBaloonGrassAll : AnimationNode, IId
    {
        public const string ID = "chapter5/McBaloonGrassAll";

        public Sprite instance5244430 { get; protected set; }

        public string Id => "chapter5/McBaloonGrassAll";

        public McBaloonGrassAll()
            : base("chapter5/McBaloonGrassAll")
        {
            instance5244430 = new Sprite(ClipIds.Chapter5.McRotatorGrass0);
            AddChild("instance5244430", instance5244430);
            Initialize();
        }
    }
}
