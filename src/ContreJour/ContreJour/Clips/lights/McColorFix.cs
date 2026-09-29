using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McColorFix : Sprite, IFreeable, IId
{
    public const string ID = "lights/McColorFix";

    public string Id => "lights/McColorFix";

    public static McColorFix New()
    {
        McColorFix mcColorFix = StaticPool.New<McColorFix>();
        mcColorFix.RefreshProperties();
        return mcColorFix;
    }

    public McColorFix()
        : base("lights/McColorFix")
    {
    }

    public void Free()
    {
        StaticPool.Free<McColorFix>(this);
    }
}
