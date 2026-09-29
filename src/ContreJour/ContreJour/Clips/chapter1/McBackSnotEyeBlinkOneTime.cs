using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackSnotEyeBlinkOneTime : MovieClip, IFreeable, IId
{
    public const string ID = "chapter1/McBackSnotEyeBlinkOneTime";

    public string Id => "chapter1/McBackSnotEyeBlinkOneTime";

    public static McBackSnotEyeBlinkOneTime New()
    {
        McBackSnotEyeBlinkOneTime mcBackSnotEyeBlinkOneTime = StaticPool.New<McBackSnotEyeBlinkOneTime>();
        mcBackSnotEyeBlinkOneTime.RefreshProperties();
        return mcBackSnotEyeBlinkOneTime;
    }

    public McBackSnotEyeBlinkOneTime()
        : base("chapter1/McBackSnotEyeBlinkOneTime")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackSnotEyeBlinkOneTime>(this);
    }
}
