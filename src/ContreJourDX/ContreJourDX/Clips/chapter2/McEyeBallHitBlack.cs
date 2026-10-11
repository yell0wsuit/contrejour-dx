using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallHitBlack : AnimationNode, IId
    {
        public const string ID = "chapter2/McEyeBallHitBlack";

        public Sprite instance24962 { get; protected set; }

        public Sprite instance24968 { get; protected set; }

        public string Id => "chapter2/McEyeBallHitBlack";

        public McEyeBallHitBlack()
            : base("chapter2/McEyeBallHitBlack")
        {
            instance24962 = new Sprite(ClipIds.Chapter2.McEyeBallBlack);
            AddChild("instance24962", instance24962);
            instance24968 = new Sprite(ClipIds.Chapter2.McEyeBallBlack);
            AddChild("instance24968", instance24968);
            Initialize();
        }
    }
}
