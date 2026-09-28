using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter3Blur : Sprite, IFreeable, IId
{
    public const string ID = "planets/McChapter3Blur";

    public string Id => "planets/McChapter3Blur";

    public static McChapter3Blur New()
    {
        McChapter3Blur mcChapter3Blur = StaticPool<McChapter3Blur>.New();
        mcChapter3Blur.RefreshProperties();
        return mcChapter3Blur;
    }

    public McChapter3Blur()
        : base("planets/McChapter3Blur")
    {
    }

    public void Free()
    {
        StaticPool<McChapter3Blur>.Free(this);
    }
}
