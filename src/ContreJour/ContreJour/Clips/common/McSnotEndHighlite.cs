using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotEndHighlite : Sprite, IFreeable, IId
{
    public const string ID = "common/McSnotEndHighlite";

    public string Id => "common/McSnotEndHighlite";

    public static McSnotEndHighlite New()
    {
        McSnotEndHighlite mcSnotEndHighlite = StaticPool.New<McSnotEndHighlite>();
        mcSnotEndHighlite.RefreshProperties();
        return mcSnotEndHighlite;
    }

    public McSnotEndHighlite()
        : base("common/McSnotEndHighlite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSnotEndHighlite>(this);
    }
}
