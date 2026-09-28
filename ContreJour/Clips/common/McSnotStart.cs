using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotStart : Sprite, IFreeable, IId
{
    public const string ID = "common/McSnotStart";

    public string Id => "common/McSnotStart";

    public static McSnotStart New()
    {
        McSnotStart mcSnotStart = StaticPool<McSnotStart>.New();
        mcSnotStart.RefreshProperties();
        return mcSnotStart;
    }

    public McSnotStart()
        : base("common/McSnotStart")
    {
    }

    public void Free()
    {
        StaticPool<McSnotStart>.Free(this);
    }
}
