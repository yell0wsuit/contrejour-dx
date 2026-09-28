using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground1Front : Sprite, IFreeable, IId
{
    public const string ID = "level1/McBackground1Front";

    public string Id => "level1/McBackground1Front";

    public static McBackground1Front New()
    {
        McBackground1Front mcBackground1Front = StaticPool<McBackground1Front>.New();
        mcBackground1Front.RefreshProperties();
        return mcBackground1Front;
    }

    public McBackground1Front()
        : base("level1/McBackground1Front")
    {
    }

    public void Free()
    {
        StaticPool<McBackground1Front>.Free(this);
    }
}
