using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEnergyView : Sprite, IFreeable, IId
{
    public const string ID = "common/McEnergyView";

    public string Id => "common/McEnergyView";

    public static McEnergyView New()
    {
        McEnergyView mcEnergyView = StaticPool<McEnergyView>.New();
        mcEnergyView.RefreshProperties();
        return mcEnergyView;
    }

    public McEnergyView()
        : base("common/McEnergyView")
    {
    }

    public void Free()
    {
        StaticPool<McEnergyView>.Free(this);
    }
}
