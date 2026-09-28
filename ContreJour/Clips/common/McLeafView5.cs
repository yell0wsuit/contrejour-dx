using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeafView5 : Sprite, IFreeable, IId
{
    public const string ID = "common/McLeafView5";

    public string Id => "common/McLeafView5";

    public static McLeafView5 New()
    {
        McLeafView5 mcLeafView = StaticPool<McLeafView5>.New();
        mcLeafView.RefreshProperties();
        return mcLeafView;
    }

    public McLeafView5()
        : base("common/McLeafView5")
    {
    }

    public void Free()
    {
        StaticPool<McLeafView5>.Free(this);
    }
}
