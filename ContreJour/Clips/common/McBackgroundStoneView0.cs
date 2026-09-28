using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView0 : Sprite, IFreeable, IId
{
    public const string ID = "common/McBackgroundStoneView0";

    public string Id => "common/McBackgroundStoneView0";

    public static McBackgroundStoneView0 New()
    {
        McBackgroundStoneView0 mcBackgroundStoneView = StaticPool.New<McBackgroundStoneView0>();
        mcBackgroundStoneView.RefreshProperties();
        return mcBackgroundStoneView;
    }

    public McBackgroundStoneView0()
        : base("common/McBackgroundStoneView0")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundStoneView0>(this);
    }
}
