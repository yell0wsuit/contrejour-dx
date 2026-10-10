using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter4Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent4_5 : AnimationNode, IId
    {
        public const string ID = "chapter4Backgrounds/McBackgroundContent4_5";

        public Sprite instance5245629 { get; protected set; }

        public string Id => "chapter4Backgrounds/McBackgroundContent4_5";

        public McBackgroundContent4_5()
            : base("chapter4Backgrounds/McBackgroundContent4_5")
        {
            instance5245629 = new Sprite(ClipIds.Chapter4Backgrounds.McBackgroundContent4_7);
            AddChild("instance5245629", instance5245629);
            Initialize();
        }
    }
}
