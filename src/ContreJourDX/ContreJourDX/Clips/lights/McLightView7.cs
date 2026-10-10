using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.lights
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLightView7 : AnimationNode, IId
    {
        public const string ID = "lights/McLightView7";

        public Sprite instance5230392 { get; protected set; }

        public string Id => "lights/McLightView7";

        public McLightView7()
            : base("lights/McLightView7")
        {
            instance5230392 = new Sprite(ClipIds.Lights.McLightView7Content);
            AddChild("instance5230392", instance5230392);
            Initialize();
        }
    }
}
