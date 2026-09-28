using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundCrabContent : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McBackgroundCrabContent";

    public string Id => "chapter1/McBackgroundCrabContent";

    public static McBackgroundCrabContent New()
    {
        McBackgroundCrabContent mcBackgroundCrabContent = StaticPool<McBackgroundCrabContent>.New();
        mcBackgroundCrabContent.RefreshProperties();
        return mcBackgroundCrabContent;
    }

    public McBackgroundCrabContent()
        : base("chapter1/McBackgroundCrabContent")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundCrabContent>.Free(this);
    }
}
