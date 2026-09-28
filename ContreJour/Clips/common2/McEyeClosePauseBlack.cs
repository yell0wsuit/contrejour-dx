using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeClosePauseBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeClosePauseBlack";

    public string Id => "common2/McEyeClosePauseBlack";

    public static McEyeClosePauseBlack New()
    {
        McEyeClosePauseBlack mcEyeClosePauseBlack = StaticPool<McEyeClosePauseBlack>.New();
        mcEyeClosePauseBlack.RefreshProperties();
        return mcEyeClosePauseBlack;
    }

    public McEyeClosePauseBlack()
        : base("common2/McEyeClosePauseBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeClosePauseBlack>.Free(this);
    }
}
