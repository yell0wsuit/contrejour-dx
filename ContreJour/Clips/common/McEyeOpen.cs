using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeOpen : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeOpen";

    public string Id => "common/McEyeOpen";

    public static McEyeOpen New()
    {
        McEyeOpen mcEyeOpen = StaticPool<McEyeOpen>.New();
        mcEyeOpen.RefreshProperties();
        return mcEyeOpen;
    }

    public McEyeOpen()
        : base("common/McEyeOpen")
    {
    }

    public void Free()
    {
        StaticPool<McEyeOpen>.Free(this);
    }
}
