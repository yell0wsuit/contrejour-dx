using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeCloseSlowBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeCloseSlowBlack";

    public string Id => "common2/McEyeCloseSlowBlack";

    public static McEyeCloseSlowBlack New()
    {
        McEyeCloseSlowBlack mcEyeCloseSlowBlack = StaticPool<McEyeCloseSlowBlack>.New();
        mcEyeCloseSlowBlack.RefreshProperties();
        return mcEyeCloseSlowBlack;
    }

    public McEyeCloseSlowBlack()
        : base("common2/McEyeCloseSlowBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeCloseSlowBlack>.Free(this);
    }
}
