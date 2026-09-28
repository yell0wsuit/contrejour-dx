using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTotalLine : MovieClip, IFreeable, IId
{
    public const string ID = "menu/McTotalLine";

    public string Id => "menu/McTotalLine";

    public static McTotalLine New()
    {
        McTotalLine mcTotalLine = StaticPool<McTotalLine>.New();
        mcTotalLine.RefreshProperties();
        return mcTotalLine;
    }

    public McTotalLine()
        : base("menu/McTotalLine")
    {
    }

    public void Free()
    {
        StaticPool<McTotalLine>.Free(this);
    }
}
