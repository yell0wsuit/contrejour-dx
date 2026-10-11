using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallHitWhite : AnimationNode, IId
    {
        public const string ID = "chapter4/McEyeBallHitWhite";

        public Sprite instance5248083 { get; protected set; }

        public Sprite instance5248089 { get; protected set; }

        public string Id => "chapter4/McEyeBallHitWhite";

        public McEyeBallHitWhite()
            : base("chapter4/McEyeBallHitWhite")
        {
            instance5248083 = new Sprite(ClipIds.Chapter4.McEyeBallWhite);
            AddChild("instance5248083", instance5248083);
            instance5248089 = new Sprite(ClipIds.Chapter4.McEyeBallWhite);
            AddChild("instance5248089", instance5248089);
            Initialize();
        }
    }
}
