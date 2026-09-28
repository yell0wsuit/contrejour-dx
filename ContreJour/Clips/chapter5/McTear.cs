using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTear : MovieClip, IFreeable, IId
{
    public const string ID = "chapter5/McTear";

    public string Id => "chapter5/McTear";

    public static McTear New()
    {
        McTear mcTear = StaticPool<McTear>.New();
        mcTear.RefreshProperties();
        return mcTear;
    }

    public McTear()
        : base("chapter5/McTear")
    {
    }

    public void Free()
    {
        StaticPool<McTear>.Free(this);
    }
}
