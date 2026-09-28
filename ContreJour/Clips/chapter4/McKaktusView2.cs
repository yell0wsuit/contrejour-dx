using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView2 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McKaktusView2";

    public string Id => "chapter4/McKaktusView2";

    public static McKaktusView2 New()
    {
        McKaktusView2 mcKaktusView = StaticPool<McKaktusView2>.New();
        mcKaktusView.RefreshProperties();
        return mcKaktusView;
    }

    public McKaktusView2()
        : base("chapter4/McKaktusView2")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView2>.Free(this);
    }
}
