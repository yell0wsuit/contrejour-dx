using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeSmileBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeSmileBlack";

    public string Id => "common2/McEyeSmileBlack";

    public static McEyeSmileBlack New()
    {
        McEyeSmileBlack mcEyeSmileBlack = StaticPool<McEyeSmileBlack>.New();
        mcEyeSmileBlack.RefreshProperties();
        return mcEyeSmileBlack;
    }

    public McEyeSmileBlack()
        : base("common2/McEyeSmileBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeSmileBlack>.Free(this);
    }
}
