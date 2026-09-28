using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTubusEyeBackground : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McTubusEyeBackground";

    public string Id => "chapter5/McTubusEyeBackground";

    public static McTubusEyeBackground New()
    {
        McTubusEyeBackground mcTubusEyeBackground = StaticPool<McTubusEyeBackground>.New();
        mcTubusEyeBackground.RefreshProperties();
        return mcTubusEyeBackground;
    }

    public McTubusEyeBackground()
        : base("chapter5/McTubusEyeBackground")
    {
    }

    public void Free()
    {
        StaticPool<McTubusEyeBackground>.Free(this);
    }
}
