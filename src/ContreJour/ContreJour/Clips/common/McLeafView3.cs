using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLeafView3 : Sprite, IFreeable, IId
{
    public const string ID = "common/McLeafView3";

    public string Id => "common/McLeafView3";

    public static McLeafView3 New()
    {
        McLeafView3 mcLeafView = StaticPool.New<McLeafView3>();
        mcLeafView.RefreshProperties();
        return mcLeafView;
    }

    public McLeafView3()
        : base("common/McLeafView3")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLeafView3>(this);
    }
}
