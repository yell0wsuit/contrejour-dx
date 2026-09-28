using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringView_5 : MovieClip, IFreeable, IId
{
    public const string ID = "common/McSpringView_5";

    public string Id => "common/McSpringView_5";

    public static McSpringView_5 New()
    {
        McSpringView_5 mcSpringView_ = StaticPool<McSpringView_5>.New();
        mcSpringView_.RefreshProperties();
        return mcSpringView_;
    }

    public McSpringView_5()
        : base("common/McSpringView_5")
    {
    }

    public void Free()
    {
        StaticPool<McSpringView_5>.Free(this);
    }
}
