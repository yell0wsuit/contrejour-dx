using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTrampolineEndBlack : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McTrampolineEndBlack";

    public string Id => "chapter2/McTrampolineEndBlack";

    public static McTrampolineEndBlack New()
    {
        McTrampolineEndBlack mcTrampolineEndBlack = StaticPool<McTrampolineEndBlack>.New();
        mcTrampolineEndBlack.RefreshProperties();
        return mcTrampolineEndBlack;
    }

    public McTrampolineEndBlack()
        : base("chapter2/McTrampolineEndBlack")
    {
    }

    public void Free()
    {
        StaticPool<McTrampolineEndBlack>.Free(this);
    }
}
