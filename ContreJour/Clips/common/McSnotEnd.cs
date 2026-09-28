using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotEnd : Sprite, IFreeable, IId
{
    public const string ID = "common/McSnotEnd";

    public string Id => "common/McSnotEnd";

    public static McSnotEnd New()
    {
        McSnotEnd mcSnotEnd = StaticPool<McSnotEnd>.New();
        mcSnotEnd.RefreshProperties();
        return mcSnotEnd;
    }

    public McSnotEnd()
        : base("common/McSnotEnd")
    {
    }

    public void Free()
    {
        StaticPool<McSnotEnd>.Free(this);
    }
}
