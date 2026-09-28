using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeg : Sprite, IFreeable, IId
{
    public const string ID = "loading/McLeg";

    public string Id => "loading/McLeg";

    public static McLeg New()
    {
        McLeg mcLeg = StaticPool.New<McLeg>();
        mcLeg.RefreshProperties();
        return mcLeg;
    }

    public McLeg()
        : base("loading/McLeg")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLeg>(this);
    }
}
