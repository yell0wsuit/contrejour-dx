using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView1 : Sprite, IFreeable, IId
{
    public const string ID = "common/McBackgroundStoneView1";

    public string Id => "common/McBackgroundStoneView1";

    public static McBackgroundStoneView1 New()
    {
        McBackgroundStoneView1 mcBackgroundStoneView = StaticPool<McBackgroundStoneView1>.New();
        mcBackgroundStoneView.RefreshProperties();
        return mcBackgroundStoneView;
    }

    public McBackgroundStoneView1()
        : base("common/McBackgroundStoneView1")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundStoneView1>.Free(this);
    }
}
