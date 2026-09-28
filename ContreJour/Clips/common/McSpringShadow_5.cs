using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringShadow_5 : Sprite, IFreeable, IId
{
    public const string ID = "common/McSpringShadow_5";

    public string Id => "common/McSpringShadow_5";

    public static McSpringShadow_5 New()
    {
        McSpringShadow_5 mcSpringShadow_ = StaticPool<McSpringShadow_5>.New();
        mcSpringShadow_.RefreshProperties();
        return mcSpringShadow_;
    }

    public McSpringShadow_5()
        : base("common/McSpringShadow_5")
    {
    }

    public void Free()
    {
        StaticPool<McSpringShadow_5>.Free(this);
    }
}
