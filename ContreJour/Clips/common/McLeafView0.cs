using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeafView0 : Sprite, IFreeable, IId
{
    public const string ID = "common/McLeafView0";

    public string Id => "common/McLeafView0";

    public static McLeafView0 New()
    {
        McLeafView0 mcLeafView = StaticPool<McLeafView0>.New();
        mcLeafView.RefreshProperties();
        return mcLeafView;
    }

    public McLeafView0()
        : base("common/McLeafView0")
    {
    }

    public void Free()
    {
        StaticPool<McLeafView0>.Free(this);
    }
}
