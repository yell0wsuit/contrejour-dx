using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFinishPart : Sprite, IFreeable, IId
{
    public const string ID = "common/McFinishPart";

    public string Id => "common/McFinishPart";

    public static McFinishPart New()
    {
        McFinishPart mcFinishPart = StaticPool<McFinishPart>.New();
        mcFinishPart.RefreshProperties();
        return mcFinishPart;
    }

    public McFinishPart()
        : base("common/McFinishPart")
    {
    }

    public void Free()
    {
        StaticPool<McFinishPart>.Free(this);
    }
}
