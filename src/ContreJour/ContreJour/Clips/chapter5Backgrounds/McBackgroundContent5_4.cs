using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent5_4 : AnimationNode, IId
    {
        public const string ID = "chapter5Backgrounds/McBackgroundContent5_4";

        public Sprite instance5242444 { get; protected set; }

        public string Id => "chapter5Backgrounds/McBackgroundContent5_4";

        public McBackgroundContent5_4()
            : base("chapter5Backgrounds/McBackgroundContent5_4")
        {
            instance5242444 = new Sprite(ClipIds.Chapter5Backgrounds.McBackgroundContent5_6);
            AddChild("instance5242444", instance5242444);
            Initialize();
        }
    }
}
