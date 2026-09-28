using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotEndBlack : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McSnotEndBlack";

    public string Id => "chapter2/McSnotEndBlack";

    public static McSnotEndBlack New()
    {
        McSnotEndBlack mcSnotEndBlack = StaticPool<McSnotEndBlack>.New();
        mcSnotEndBlack.RefreshProperties();
        return mcSnotEndBlack;
    }

    public McSnotEndBlack()
        : base("chapter2/McSnotEndBlack")
    {
    }

    public void Free()
    {
        StaticPool<McSnotEndBlack>.Free(this);
    }
}
