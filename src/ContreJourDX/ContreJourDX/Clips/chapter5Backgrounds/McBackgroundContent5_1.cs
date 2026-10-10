using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter5Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent5_1 : AnimationNode, IId
    {
        public const string ID = "chapter5Backgrounds/McBackgroundContent5_1";

        public Sprite instance5242426 { get; protected set; }

        public string Id => "chapter5Backgrounds/McBackgroundContent5_1";

        public McBackgroundContent5_1()
            : base("chapter5Backgrounds/McBackgroundContent5_1")
        {
            instance5242426 = new Sprite(ClipIds.Chapter5Backgrounds.McBackgroundContent5_3);
            AddChild("instance5242426", instance5242426);
            Initialize();
        }
    }
}
