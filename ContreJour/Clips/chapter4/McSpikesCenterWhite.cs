using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesCenterWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McSpikesCenterWhite";

    public string Id => "chapter4/McSpikesCenterWhite";

    public static McSpikesCenterWhite New()
    {
        McSpikesCenterWhite mcSpikesCenterWhite = StaticPool<McSpikesCenterWhite>.New();
        mcSpikesCenterWhite.RefreshProperties();
        return mcSpikesCenterWhite;
    }

    public McSpikesCenterWhite()
        : base("chapter4/McSpikesCenterWhite")
    {
    }

    public void Free()
    {
        StaticPool<McSpikesCenterWhite>.Free(this);
    }
}
