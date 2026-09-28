using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeSmile : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeSmile";

    public string Id => "common/McEyeSmile";

    public static McEyeSmile New()
    {
        McEyeSmile mcEyeSmile = StaticPool<McEyeSmile>.New();
        mcEyeSmile.RefreshProperties();
        return mcEyeSmile;
    }

    public McEyeSmile()
        : base("common/McEyeSmile")
    {
    }

    public void Free()
    {
        StaticPool<McEyeSmile>.Free(this);
    }
}
