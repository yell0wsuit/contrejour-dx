using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStrongSnotEndWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McStrongSnotEndWhite";

    public string Id => "chapter4/McStrongSnotEndWhite";

    public static McStrongSnotEndWhite New()
    {
        McStrongSnotEndWhite mcStrongSnotEndWhite = StaticPool.New<McStrongSnotEndWhite>();
        mcStrongSnotEndWhite.RefreshProperties();
        return mcStrongSnotEndWhite;
    }

    public McStrongSnotEndWhite()
        : base("chapter4/McStrongSnotEndWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McStrongSnotEndWhite>(this);
    }
}
