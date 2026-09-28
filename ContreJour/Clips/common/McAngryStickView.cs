using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McAngryStickView : Sprite, IFreeable, IId
{
    public const string ID = "common/McAngryStickView";

    public string Id => "common/McAngryStickView";

    public static McAngryStickView New()
    {
        McAngryStickView mcAngryStickView = StaticPool<McAngryStickView>.New();
        mcAngryStickView.RefreshProperties();
        return mcAngryStickView;
    }

    public McAngryStickView()
        : base("common/McAngryStickView")
    {
    }

    public void Free()
    {
        StaticPool<McAngryStickView>.Free(this);
    }
}
