using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeDeadBlack : Sprite, IFreeable, IId
{
    public const string ID = "common2/McEyeDeadBlack";

    public string Id => "common2/McEyeDeadBlack";

    public static McEyeDeadBlack New()
    {
        McEyeDeadBlack mcEyeDeadBlack = StaticPool<McEyeDeadBlack>.New();
        mcEyeDeadBlack.RefreshProperties();
        return mcEyeDeadBlack;
    }

    public McEyeDeadBlack()
        : base("common2/McEyeDeadBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeDeadBlack>.Free(this);
    }
}
