using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorPoint : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorPoint";

    public string Id => "chapter5/McRotatorPoint";

    public static McRotatorPoint New()
    {
        McRotatorPoint mcRotatorPoint = StaticPool.New<McRotatorPoint>();
        mcRotatorPoint.RefreshProperties();
        return mcRotatorPoint;
    }

    public McRotatorPoint()
        : base("chapter5/McRotatorPoint")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRotatorPoint>(this);
    }
}
