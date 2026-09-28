using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView3Black : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McKaktusView3Black";

    public string Id => "chapter2/McKaktusView3Black";

    public static McKaktusView3Black New()
    {
        McKaktusView3Black mcKaktusView3Black = StaticPool<McKaktusView3Black>.New();
        mcKaktusView3Black.RefreshProperties();
        return mcKaktusView3Black;
    }

    public McKaktusView3Black()
        : base("chapter2/McKaktusView3Black")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView3Black>.Free(this);
    }
}
