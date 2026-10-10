using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.lights
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLightView12 : AnimationNode, IId
    {
        public const string ID = "lights/McLightView12";

        public Sprite instance5230352 { get; protected set; }

        public string Id => "lights/McLightView12";

        public McLightView12()
            : base("lights/McLightView12")
        {
            instance5230352 = new Sprite(ClipIds.Lights.McLightView11ContentExport);
            AddChild("instance5230352", instance5230352);
            Initialize();
        }
    }
}
