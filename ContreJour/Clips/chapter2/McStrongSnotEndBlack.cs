using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStrongSnotEndBlack : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McStrongSnotEndBlack";

    public string Id => "chapter2/McStrongSnotEndBlack";

    public static McStrongSnotEndBlack New()
    {
        McStrongSnotEndBlack mcStrongSnotEndBlack = StaticPool.New<McStrongSnotEndBlack>();
        mcStrongSnotEndBlack.RefreshProperties();
        return mcStrongSnotEndBlack;
    }

    public McStrongSnotEndBlack()
        : base("chapter2/McStrongSnotEndBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McStrongSnotEndBlack>(this);
    }
}
