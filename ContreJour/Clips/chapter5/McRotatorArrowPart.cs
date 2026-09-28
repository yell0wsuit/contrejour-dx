using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorArrowPart : MovieClip, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorArrowPart";

    public string Id => "chapter5/McRotatorArrowPart";

    public static McRotatorArrowPart New()
    {
        McRotatorArrowPart mcRotatorArrowPart = StaticPool<McRotatorArrowPart>.New();
        mcRotatorArrowPart.RefreshProperties();
        return mcRotatorArrowPart;
    }

    public McRotatorArrowPart()
        : base("chapter5/McRotatorArrowPart")
    {
    }

    public void Free()
    {
        StaticPool<McRotatorArrowPart>.Free(this);
    }
}
