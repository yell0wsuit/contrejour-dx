using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetStick : AnimationNode, IId
    {
        public const string ID = "planets/McPlanetStick";

        public McPlanetStickGraphics content { get; protected set; }

        public string Id => "planets/McPlanetStick";

        public McPlanetStick()
            : base("planets/McPlanetStick")
        {
            content = new McPlanetStickGraphics();
            AddChild("content", content);
            Initialize();
        }
    }
}
