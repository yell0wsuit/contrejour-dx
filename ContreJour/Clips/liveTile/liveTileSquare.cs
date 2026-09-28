using System.CodeDom.Compiler;
using ContreJour.WinRT;
using Microsoft.Xna.Framework;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual.Text;

namespace ContreJour.Clips.liveTile;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class liveTileSquare : LiveTileAnimation, IFreeable, IId
{
    public const string ID = "liveTile/liveTileSquare";

    public blackSquare instance640 { get; protected set; }

    public liveTileGraphics_358x358 background { get; protected set; }

    public liveTileBlueLight blueLight { get; protected set; }

    public liveTileHighLight highLight { get; protected set; }

    public Label count { get; protected set; }

    public string Id => "liveTile/liveTileSquare";

    public static liveTileSquare New()
    {
        liveTileSquare liveTileSquare2 = StaticPool<liveTileSquare>.New();
        liveTileSquare2.RefreshProperties();
        return liveTileSquare2;
    }

    public liveTileSquare()
        : base("liveTile/liveTileSquare")
    {
        instance640 = new blackSquare();
        AddChild("instance640", instance640);
        background = new liveTileGraphics_358x358();
        AddChild("background", background);
        blueLight = new liveTileBlueLight();
        AddChild("blueLight", blueLight);
        highLight = new liveTileHighLight();
        AddChild("highLight", highLight);
        count = new Label("Segoe Print", 28f, new Vector2(72f, 59.75f))
        {
            TextString = "5",
            LineSpacing = 2f,
            Align = TextAlign.Right
        };
        AddChild("count", count);
        Initialize();
    }

    public void Free()
    {
        StaticPool<liveTileSquare>.Free(this);
    }
}
