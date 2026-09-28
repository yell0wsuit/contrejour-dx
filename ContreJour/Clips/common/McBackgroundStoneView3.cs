using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView3 : Sprite, IFreeable, IId
{
    public const string ID = "common/McBackgroundStoneView3";

    public string Id => "common/McBackgroundStoneView3";

    public static McBackgroundStoneView3 New()
    {
        McBackgroundStoneView3 mcBackgroundStoneView = StaticPool.New<McBackgroundStoneView3>();
        mcBackgroundStoneView.RefreshProperties();
        return mcBackgroundStoneView;
    }

    public McBackgroundStoneView3()
        : base("common/McBackgroundStoneView3")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundStoneView3>(this);
    }
}
