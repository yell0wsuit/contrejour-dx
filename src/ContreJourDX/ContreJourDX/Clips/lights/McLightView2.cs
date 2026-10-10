using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.lights
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLightView2 : AnimationNode, IId
    {
        public const string ID = "lights/McLightView2";

        public Sprite instance5230372 { get; protected set; }

        public string Id => "lights/McLightView2";

        public McLightView2()
            : base("lights/McLightView2")
        {
            instance5230372 = new Sprite(ClipIds.Lights.McLightView7Content);
            AddChild("instance5230372", instance5230372);
            Initialize();
        }
    }
}
