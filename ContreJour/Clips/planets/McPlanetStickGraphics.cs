using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetStickGraphics : AnimationNode, IFreeable, IId
{
    public const string ID = "planets/McPlanetStickGraphics";

    public McPlanetStickContent instance20687151 { get; protected set; }

    public McPlanetStickBall instance20687153 { get; protected set; }

    public McPlanetStickBall instance20687155 { get; protected set; }

    public string Id => "planets/McPlanetStickGraphics";

    public static McPlanetStickGraphics New()
    {
        McPlanetStickGraphics mcPlanetStickGraphics = StaticPool<McPlanetStickGraphics>.New();
        mcPlanetStickGraphics.RefreshProperties();
        return mcPlanetStickGraphics;
    }

    public McPlanetStickGraphics()
        : base("planets/McPlanetStickGraphics")
    {
        instance20687151 = new McPlanetStickContent();
        AddChild("instance20687151", instance20687151);
        instance20687153 = new McPlanetStickBall();
        AddChild("instance20687153", instance20687153);
        instance20687155 = new McPlanetStickBall();
        AddChild("instance20687155", instance20687155);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McPlanetStickGraphics>.Free(this);
    }
}
