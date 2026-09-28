using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadBack : Sprite, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadBack";

    public string Id => "level1/McRoseHeadBack";

    public static McRoseHeadBack New()
    {
        McRoseHeadBack mcRoseHeadBack = StaticPool<McRoseHeadBack>.New();
        mcRoseHeadBack.RefreshProperties();
        return mcRoseHeadBack;
    }

    public McRoseHeadBack()
        : base("level1/McRoseHeadBack")
    {
    }

    public void Free()
    {
        StaticPool<McRoseHeadBack>.Free(this);
    }
}
