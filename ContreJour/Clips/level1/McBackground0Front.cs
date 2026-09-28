using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground0Front : Sprite, IFreeable, IId
{
    public const string ID = "level1/McBackground0Front";

    public string Id => "level1/McBackground0Front";

    public static McBackground0Front New()
    {
        McBackground0Front mcBackground0Front = StaticPool.New<McBackground0Front>();
        mcBackground0Front.RefreshProperties();
        return mcBackground0Front;
    }

    public McBackground0Front()
        : base("level1/McBackground0Front")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackground0Front>(this);
    }
}
