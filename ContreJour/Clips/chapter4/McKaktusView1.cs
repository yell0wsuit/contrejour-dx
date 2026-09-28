using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView1 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McKaktusView1";

    public string Id => "chapter4/McKaktusView1";

    public static McKaktusView1 New()
    {
        McKaktusView1 mcKaktusView = StaticPool<McKaktusView1>.New();
        mcKaktusView.RefreshProperties();
        return mcKaktusView;
    }

    public McKaktusView1()
        : base("chapter4/McKaktusView1")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView1>.Free(this);
    }
}
