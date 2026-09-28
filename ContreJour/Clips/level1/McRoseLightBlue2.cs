using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseLightBlue2 : Sprite, IFreeable, IId
{
    public const string ID = "level1/McRoseLightBlue2";

    public string Id => "level1/McRoseLightBlue2";

    public static McRoseLightBlue2 New()
    {
        McRoseLightBlue2 mcRoseLightBlue = StaticPool<McRoseLightBlue2>.New();
        mcRoseLightBlue.RefreshProperties();
        return mcRoseLightBlue;
    }

    public McRoseLightBlue2()
        : base("level1/McRoseLightBlue2")
    {
    }

    public void Free()
    {
        StaticPool<McRoseLightBlue2>.Free(this);
    }
}
