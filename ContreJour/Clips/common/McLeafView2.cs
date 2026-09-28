using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeafView2 : Sprite, IFreeable, IId
{
    public const string ID = "common/McLeafView2";

    public string Id => "common/McLeafView2";

    public static McLeafView2 New()
    {
        McLeafView2 mcLeafView = StaticPool<McLeafView2>.New();
        mcLeafView.RefreshProperties();
        return mcLeafView;
    }

    public McLeafView2()
        : base("common/McLeafView2")
    {
    }

    public void Free()
    {
        StaticPool<McLeafView2>.Free(this);
    }
}
