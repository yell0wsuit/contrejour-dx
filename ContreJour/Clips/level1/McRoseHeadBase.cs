using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadBase : Sprite, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadBase";

    public string Id => "level1/McRoseHeadBase";

    public static McRoseHeadBase New()
    {
        McRoseHeadBase mcRoseHeadBase = StaticPool<McRoseHeadBase>.New();
        mcRoseHeadBase.RefreshProperties();
        return mcRoseHeadBase;
    }

    public McRoseHeadBase()
        : base("level1/McRoseHeadBase")
    {
    }

    public void Free()
    {
        StaticPool<McRoseHeadBase>.Free(this);
    }
}
