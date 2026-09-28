using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeCloseBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeCloseBlack";

    public string Id => "common2/McEyeCloseBlack";

    public static McEyeCloseBlack New()
    {
        McEyeCloseBlack mcEyeCloseBlack = StaticPool<McEyeCloseBlack>.New();
        mcEyeCloseBlack.RefreshProperties();
        return mcEyeCloseBlack;
    }

    public McEyeCloseBlack()
        : base("common2/McEyeCloseBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeCloseBlack>.Free(this);
    }
}
