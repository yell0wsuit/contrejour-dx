using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class PelustokNoLight : Sprite, IFreeable, IId
{
    public const string ID = "level1/PelustokNoLight";

    public string Id => "level1/PelustokNoLight";

    public static PelustokNoLight New()
    {
        PelustokNoLight pelustokNoLight = StaticPool.New<PelustokNoLight>();
        pelustokNoLight.RefreshProperties();
        return pelustokNoLight;
    }

    public PelustokNoLight()
        : base("level1/PelustokNoLight")
    {
    }

    public void Free()
    {
        StaticPool.Free<PelustokNoLight>(this);
    }
}
