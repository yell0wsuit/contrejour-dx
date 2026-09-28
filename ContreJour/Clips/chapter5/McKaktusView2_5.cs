using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView2_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McKaktusView2_5";

    public string Id => "chapter5/McKaktusView2_5";

    public static McKaktusView2_5 New()
    {
        McKaktusView2_5 mcKaktusView2_ = StaticPool<McKaktusView2_5>.New();
        mcKaktusView2_.RefreshProperties();
        return mcKaktusView2_;
    }

    public McKaktusView2_5()
        : base("chapter5/McKaktusView2_5")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView2_5>.Free(this);
    }
}
