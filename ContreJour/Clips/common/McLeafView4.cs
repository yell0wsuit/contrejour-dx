using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeafView4 : Sprite, IFreeable, IId
{
    public const string ID = "common/McLeafView4";

    public string Id => "common/McLeafView4";

    public static McLeafView4 New()
    {
        McLeafView4 mcLeafView = StaticPool<McLeafView4>.New();
        mcLeafView.RefreshProperties();
        return mcLeafView;
    }

    public McLeafView4()
        : base("common/McLeafView4")
    {
    }

    public void Free()
    {
        StaticPool<McLeafView4>.Free(this);
    }
}
