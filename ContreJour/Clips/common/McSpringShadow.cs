using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringShadow : Sprite, IFreeable, IId
{
    public const string ID = "common/McSpringShadow";

    public string Id => "common/McSpringShadow";

    public static McSpringShadow New()
    {
        McSpringShadow mcSpringShadow = StaticPool.New<McSpringShadow>();
        mcSpringShadow.RefreshProperties();
        return mcSpringShadow;
    }

    public McSpringShadow()
        : base("common/McSpringShadow")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSpringShadow>(this);
    }
}
