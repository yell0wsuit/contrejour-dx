using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTear : MovieClip, IFreeable, IId
{
    public const string ID = "level1/McTear";

    public string Id => "level1/McTear";

    public static McTear New()
    {
        McTear mcTear = StaticPool.New<McTear>();
        mcTear.RefreshProperties();
        return mcTear;
    }

    public McTear()
        : base("level1/McTear")
    {
    }

    public void Free()
    {
        StaticPool.Free<McTear>(this);
    }
}
