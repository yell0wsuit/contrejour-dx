using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadBase2 : Sprite, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadBase2";

    public string Id => "level1/McRoseHeadBase2";

    public static McRoseHeadBase2 New()
    {
        McRoseHeadBase2 mcRoseHeadBase = StaticPool.New<McRoseHeadBase2>();
        mcRoseHeadBase.RefreshProperties();
        return mcRoseHeadBase;
    }

    public McRoseHeadBase2()
        : base("level1/McRoseHeadBase2")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRoseHeadBase2>(this);
    }
}
