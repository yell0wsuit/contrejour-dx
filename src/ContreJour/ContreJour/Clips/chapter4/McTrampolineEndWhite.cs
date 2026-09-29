using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTrampolineEndWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McTrampolineEndWhite";

    public string Id => "chapter4/McTrampolineEndWhite";

    public static McTrampolineEndWhite New()
    {
        McTrampolineEndWhite mcTrampolineEndWhite = StaticPool.New<McTrampolineEndWhite>();
        mcTrampolineEndWhite.RefreshProperties();
        return mcTrampolineEndWhite;
    }

    public McTrampolineEndWhite()
        : base("chapter4/McTrampolineEndWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McTrampolineEndWhite>(this);
    }
}
