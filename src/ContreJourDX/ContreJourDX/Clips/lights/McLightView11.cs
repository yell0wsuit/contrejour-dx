using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.lights
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLightView11 : AnimationNode, IId
    {
        public const string ID = "lights/McLightView11";

        public Sprite instance5230326 { get; protected set; }

        public string Id => "lights/McLightView11";

        public McLightView11()
            : base("lights/McLightView11")
        {
            instance5230326 = new Sprite(ClipIds.Lights.McLightView11ContentExport);
            AddChild("instance5230326", instance5230326);
            Initialize();
        }
    }
}
