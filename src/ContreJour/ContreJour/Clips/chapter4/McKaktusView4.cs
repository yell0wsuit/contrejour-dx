using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView4 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McKaktusView4";

    public string Id => "chapter4/McKaktusView4";

    public static McKaktusView4 New()
    {
        McKaktusView4 mcKaktusView = StaticPool.New<McKaktusView4>();
        mcKaktusView.RefreshProperties();
        return mcKaktusView;
    }

    public McKaktusView4()
        : base("chapter4/McKaktusView4")
    {
    }

    public void Free()
    {
        StaticPool.Free<McKaktusView4>(this);
    }
}
