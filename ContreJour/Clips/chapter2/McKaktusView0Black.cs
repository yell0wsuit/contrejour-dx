using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McKaktusView0Black : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McKaktusView0Black";

    public string Id => "chapter2/McKaktusView0Black";

    public static McKaktusView0Black New()
    {
        McKaktusView0Black mcKaktusView0Black = StaticPool<McKaktusView0Black>.New();
        mcKaktusView0Black.RefreshProperties();
        return mcKaktusView0Black;
    }

    public McKaktusView0Black()
        : base("chapter2/McKaktusView0Black")
    {
    }

    public void Free()
    {
        StaticPool<McKaktusView0Black>.Free(this);
    }
}
