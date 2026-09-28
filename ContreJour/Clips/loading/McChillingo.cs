using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChillingo : Sprite, IFreeable, IId
{
    public const string ID = "loading/McChillingo";

    public string Id => "loading/McChillingo";

    public static McChillingo New()
    {
        McChillingo mcChillingo = StaticPool.New<McChillingo>();
        mcChillingo.RefreshProperties();
        return mcChillingo;
    }

    public McChillingo()
        : base("loading/McChillingo")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChillingo>(this);
    }
}
