using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "common/McEyeBall";

    public string Id => "common/McEyeBall";

    public static McEyeBall New()
    {
        McEyeBall mcEyeBall = StaticPool.New<McEyeBall>();
        mcEyeBall.RefreshProperties();
        return mcEyeBall;
    }

    public McEyeBall()
        : base("common/McEyeBall")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeBall>(this);
    }
}
