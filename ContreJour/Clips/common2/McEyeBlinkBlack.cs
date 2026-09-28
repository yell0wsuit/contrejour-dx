using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlinkBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeBlinkBlack";

    public string Id => "common2/McEyeBlinkBlack";

    public static McEyeBlinkBlack New()
    {
        McEyeBlinkBlack mcEyeBlinkBlack = StaticPool<McEyeBlinkBlack>.New();
        mcEyeBlinkBlack.RefreshProperties();
        return mcEyeBlinkBlack;
    }

    public McEyeBlinkBlack()
        : base("common2/McEyeBlinkBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBlinkBlack>.Free(this);
    }
}
