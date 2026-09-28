using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRicochetView : Sprite, IFreeable, IId
{
    public const string ID = "common/McRicochetView";

    public string Id => "common/McRicochetView";

    public static McRicochetView New()
    {
        McRicochetView mcRicochetView = StaticPool.New<McRicochetView>();
        mcRicochetView.RefreshProperties();
        return mcRicochetView;
    }

    public McRicochetView()
        : base("common/McRicochetView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRicochetView>(this);
    }
}
