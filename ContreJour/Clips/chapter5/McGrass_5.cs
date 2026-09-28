using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGrass_5 : MovieClip, IFreeable, IId
{
    public const string ID = "chapter5/McGrass_5";

    public string Id => "chapter5/McGrass_5";

    public static McGrass_5 New()
    {
        McGrass_5 mcGrass_ = StaticPool<McGrass_5>.New();
        mcGrass_.RefreshProperties();
        return mcGrass_;
    }

    public McGrass_5()
        : base("chapter5/McGrass_5")
    {
    }

    public void Free()
    {
        StaticPool<McGrass_5>.Free(this);
    }
}
