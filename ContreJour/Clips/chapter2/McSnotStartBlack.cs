using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotStartBlack : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McSnotStartBlack";

    public string Id => "chapter2/McSnotStartBlack";

    public static McSnotStartBlack New()
    {
        McSnotStartBlack mcSnotStartBlack = StaticPool<McSnotStartBlack>.New();
        mcSnotStartBlack.RefreshProperties();
        return mcSnotStartBlack;
    }

    public McSnotStartBlack()
        : base("chapter2/McSnotStartBlack")
    {
    }

    public void Free()
    {
        StaticPool<McSnotStartBlack>.Free(this);
    }
}
