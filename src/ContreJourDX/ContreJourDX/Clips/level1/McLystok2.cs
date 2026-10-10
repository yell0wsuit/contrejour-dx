using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLystok2 : AnimationNode, IId
    {
        public const string ID = "level1/McLystok2";

        public Sprite instance5232095 { get; protected set; }

        public string Id => "level1/McLystok2";

        public McLystok2()
            : base("level1/McLystok2")
        {
            instance5232095 = new Sprite(ClipIds.Level1.McLystok2Content);
            AddChild("instance5232095", instance5232095);
            Initialize();
        }
    }
}
