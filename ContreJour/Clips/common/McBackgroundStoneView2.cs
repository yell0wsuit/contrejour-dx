using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView2 : Sprite, IFreeable, IId
{
    public const string ID = "common/McBackgroundStoneView2";

    public string Id => "common/McBackgroundStoneView2";

    public static McBackgroundStoneView2 New()
    {
        McBackgroundStoneView2 mcBackgroundStoneView = StaticPool.New<McBackgroundStoneView2>();
        mcBackgroundStoneView.RefreshProperties();
        return mcBackgroundStoneView;
    }

    public McBackgroundStoneView2()
        : base("common/McBackgroundStoneView2")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundStoneView2>(this);
    }
}
