using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatableEggView0 : Sprite, IFreeable, IId
{
    public const string ID = "common/McRotatableEggView0";

    public string Id => "common/McRotatableEggView0";

    public static McRotatableEggView0 New()
    {
        McRotatableEggView0 mcRotatableEggView = StaticPool.New<McRotatableEggView0>();
        mcRotatableEggView.RefreshProperties();
        return mcRotatableEggView;
    }

    public McRotatableEggView0()
        : base("common/McRotatableEggView0")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRotatableEggView0>(this);
    }
}
