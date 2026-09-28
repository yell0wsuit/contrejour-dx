using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTrampolinePath : Sprite, IFreeable, IId
{
    public const string ID = "common/McTrampolinePath";

    public string Id => "common/McTrampolinePath";

    public static McTrampolinePath New()
    {
        McTrampolinePath mcTrampolinePath = StaticPool<McTrampolinePath>.New();
        mcTrampolinePath.RefreshProperties();
        return mcTrampolinePath;
    }

    public McTrampolinePath()
        : base("common/McTrampolinePath")
    {
    }

    public void Free()
    {
        StaticPool<McTrampolinePath>.Free(this);
    }
}
