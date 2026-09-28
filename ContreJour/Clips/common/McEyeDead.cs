using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeDead : Sprite, IFreeable, IId
{
    public const string ID = "common/McEyeDead";

    public string Id => "common/McEyeDead";

    public static McEyeDead New()
    {
        McEyeDead mcEyeDead = StaticPool<McEyeDead>.New();
        mcEyeDead.RefreshProperties();
        return mcEyeDead;
    }

    public McEyeDead()
        : base("common/McEyeDead")
    {
    }

    public void Free()
    {
        StaticPool<McEyeDead>.Free(this);
    }
}
