using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter5Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent5_2 : AnimationNode, IId
    {
        public const string ID = "chapter5Backgrounds/McBackgroundContent5_2";

        public Sprite instance5242434 { get; protected set; }

        public string Id => "chapter5Backgrounds/McBackgroundContent5_2";

        public McBackgroundContent5_2()
            : base("chapter5Backgrounds/McBackgroundContent5_2")
        {
            instance5242434 = new Sprite(ClipIds.Chapter5Backgrounds.McBackgroundContent5_7);
            AddChild("instance5242434", instance5242434);
            Initialize();
        }
    }
}
