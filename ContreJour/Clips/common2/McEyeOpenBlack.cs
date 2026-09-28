using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeOpenBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeOpenBlack";

    public string Id => "common2/McEyeOpenBlack";

    public static McEyeOpenBlack New()
    {
        McEyeOpenBlack mcEyeOpenBlack = StaticPool<McEyeOpenBlack>.New();
        mcEyeOpenBlack.RefreshProperties();
        return mcEyeOpenBlack;
    }

    public McEyeOpenBlack()
        : base("common2/McEyeOpenBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeOpenBlack>.Free(this);
    }
}
