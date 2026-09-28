using System.CodeDom.Compiler;

using Mokus2D;
using Mokus2D.Visual.Text;

namespace ContreJour.Clips.segoeFont;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class SegoePrint28Label : Label
{
    public const string FontName = "SegoePrint";

    public const string ID = "segoeFont/SegoePrint28";

    public static void Register()
    {
        Mokus2DGame.RegisterFont("SegoePrint", "segoeFont/SegoePrint28");
    }

    public SegoePrint28Label()
        : base("segoeFont/SegoePrint28")
    {
    }
}
