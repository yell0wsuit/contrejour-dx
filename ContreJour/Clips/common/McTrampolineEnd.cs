using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTrampolineEnd : Sprite, IFreeable, IId
{
    public const string ID = "common/McTrampolineEnd";

    public string Id => "common/McTrampolineEnd";

    public static McTrampolineEnd New()
    {
        McTrampolineEnd mcTrampolineEnd = StaticPool<McTrampolineEnd>.New();
        mcTrampolineEnd.RefreshProperties();
        return mcTrampolineEnd;
    }

    public McTrampolineEnd()
        : base("common/McTrampolineEnd")
    {
    }

    public void Free()
    {
        StaticPool<McTrampolineEnd>.Free(this);
    }
}
