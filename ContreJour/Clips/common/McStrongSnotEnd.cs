using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStrongSnotEnd : Sprite, IFreeable, IId
{
    public const string ID = "common/McStrongSnotEnd";

    public string Id => "common/McStrongSnotEnd";

    public static McStrongSnotEnd New()
    {
        McStrongSnotEnd mcStrongSnotEnd = StaticPool<McStrongSnotEnd>.New();
        mcStrongSnotEnd.RefreshProperties();
        return mcStrongSnotEnd;
    }

    public McStrongSnotEnd()
        : base("common/McStrongSnotEnd")
    {
    }

    public void Free()
    {
        StaticPool<McStrongSnotEnd>.Free(this);
    }
}
