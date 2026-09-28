using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerHighlite : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSuckerHighlite";

    public string Id => "chapter5/McSuckerHighlite";

    public static McSuckerHighlite New()
    {
        McSuckerHighlite mcSuckerHighlite = StaticPool<McSuckerHighlite>.New();
        mcSuckerHighlite.RefreshProperties();
        return mcSuckerHighlite;
    }

    public McSuckerHighlite()
        : base("chapter5/McSuckerHighlite")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerHighlite>.Free(this);
    }
}
