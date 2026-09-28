using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringTouchPoint : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSpringTouchPoint";

    public string Id => "chapter5/McSpringTouchPoint";

    public static McSpringTouchPoint New()
    {
        McSpringTouchPoint mcSpringTouchPoint = StaticPool<McSpringTouchPoint>.New();
        mcSpringTouchPoint.RefreshProperties();
        return mcSpringTouchPoint;
    }

    public McSpringTouchPoint()
        : base("chapter5/McSpringTouchPoint")
    {
    }

    public void Free()
    {
        StaticPool<McSpringTouchPoint>.Free(this);
    }
}
