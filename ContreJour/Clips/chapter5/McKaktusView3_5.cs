using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView3_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McKaktusView3_5";

    public string Id => "chapter5/McKaktusView3_5";

    public static McKaktusView3_5 New()
    {
        McKaktusView3_5 mcKaktusView3_ = StaticPool<McKaktusView3_5>.New();
        mcKaktusView3_.RefreshProperties();
        return mcKaktusView3_;
    }

    public McKaktusView3_5()
        : base("chapter5/McKaktusView3_5")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView3_5>.Free(this);
    }
}
