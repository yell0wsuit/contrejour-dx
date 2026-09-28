using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeWinkBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeWinkBlack";

    public string Id => "common2/McEyeWinkBlack";

    public static McEyeWinkBlack New()
    {
        McEyeWinkBlack mcEyeWinkBlack = StaticPool<McEyeWinkBlack>.New();
        mcEyeWinkBlack.RefreshProperties();
        return mcEyeWinkBlack;
    }

    public McEyeWinkBlack()
        : base("common2/McEyeWinkBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeWinkBlack>.Free(this);
    }
}
