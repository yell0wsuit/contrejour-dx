using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEye : Sprite, IFreeable, IId
{
    public const string ID = "common/McEye";

    public string Id => "common/McEye";

    public static McEye New()
    {
        McEye mcEye = StaticPool.New<McEye>();
        mcEye.RefreshProperties();
        return mcEye;
    }

    public McEye()
        : base("common/McEye")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEye>(this);
    }
}
