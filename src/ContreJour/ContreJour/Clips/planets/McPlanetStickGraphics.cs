using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetStickGraphics : AnimationNode, IId
    {
        public const string ID = "planets/McPlanetStickGraphics";

        public Sprite instance20687151 { get; protected set; }

        public Sprite instance20687153 { get; protected set; }

        public Sprite instance20687155 { get; protected set; }

        public string Id => "planets/McPlanetStickGraphics";

        public McPlanetStickGraphics()
            : base("planets/McPlanetStickGraphics")
        {
            instance20687151 = new Sprite(ClipIds.Planets.McPlanetStickContent);
            AddChild("instance20687151", instance20687151);
            instance20687153 = new Sprite(ClipIds.Planets.McPlanetStickBall);
            AddChild("instance20687153", instance20687153);
            instance20687155 = new Sprite(ClipIds.Planets.McPlanetStickBall);
            AddChild("instance20687155", instance20687155);
            Initialize();
        }
    }
}
