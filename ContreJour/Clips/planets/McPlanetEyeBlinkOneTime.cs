using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetEyeBlinkOneTime : MovieClip, IFreeable, IId
{
    public const string ID = "planets/McPlanetEyeBlinkOneTime";

    public string Id => "planets/McPlanetEyeBlinkOneTime";

    public static McPlanetEyeBlinkOneTime New()
    {
        McPlanetEyeBlinkOneTime mcPlanetEyeBlinkOneTime = StaticPool<McPlanetEyeBlinkOneTime>.New();
        mcPlanetEyeBlinkOneTime.RefreshProperties();
        return mcPlanetEyeBlinkOneTime;
    }

    public McPlanetEyeBlinkOneTime()
        : base("planets/McPlanetEyeBlinkOneTime")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetEyeBlinkOneTime>.Free(this);
    }
}
