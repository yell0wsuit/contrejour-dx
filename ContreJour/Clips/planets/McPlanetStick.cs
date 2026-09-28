using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetStick : AnimationNode, IFreeable, IId
{
    public const string ID = "planets/McPlanetStick";

    public McPlanetStickGraphics content { get; protected set; }

    public string Id => "planets/McPlanetStick";

    public static McPlanetStick New()
    {
        McPlanetStick mcPlanetStick = StaticPool<McPlanetStick>.New();
        mcPlanetStick.RefreshProperties();
        return mcPlanetStick;
    }

    public McPlanetStick()
        : base("planets/McPlanetStick")
    {
        content = new McPlanetStickGraphics();
        AddChild("content", content);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McPlanetStick>.Free(this);
    }
}
