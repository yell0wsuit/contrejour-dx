using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlinkOneTimeBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeBlinkOneTimeBlack";

    public string Id => "common2/McEyeBlinkOneTimeBlack";

    public static McEyeBlinkOneTimeBlack New()
    {
        McEyeBlinkOneTimeBlack mcEyeBlinkOneTimeBlack = StaticPool.New<McEyeBlinkOneTimeBlack>();
        mcEyeBlinkOneTimeBlack.RefreshProperties();
        return mcEyeBlinkOneTimeBlack;
    }

    public McEyeBlinkOneTimeBlack()
        : base("common2/McEyeBlinkOneTimeBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeBlinkOneTimeBlack>(this);
    }
}
