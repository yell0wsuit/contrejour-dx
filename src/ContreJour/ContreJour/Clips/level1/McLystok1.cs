using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLystok1 : AnimationNode, IId
    {
        public const string ID = "level1/McLystok1";

        public Sprite instance5232087 { get; protected set; }

        public string Id => "level1/McLystok1";

        public McLystok1()
            : base("level1/McLystok1")
        {
            instance5232087 = new Sprite(ClipIds.Level1.McLystok1Content);
            AddChild("instance5232087", instance5232087);
            Initialize();
        }
    }
}
