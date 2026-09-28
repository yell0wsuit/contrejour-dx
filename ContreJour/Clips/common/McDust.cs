using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McDust : Sprite, IFreeable, IId
{
    public const string ID = "common/McDust";

    public string Id => "common/McDust";

    public static McDust New()
    {
        McDust mcDust = StaticPool.New<McDust>();
        mcDust.RefreshProperties();
        return mcDust;
    }

    public McDust()
        : base("common/McDust")
    {
    }

    public void Free()
    {
        StaticPool.Free<McDust>(this);
    }
}
