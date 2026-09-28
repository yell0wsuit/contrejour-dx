using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter2Blur : Sprite, IFreeable, IId
{
    public const string ID = "planets/McChapter2Blur";

    public string Id => "planets/McChapter2Blur";

    public static McChapter2Blur New()
    {
        McChapter2Blur mcChapter2Blur = StaticPool<McChapter2Blur>.New();
        mcChapter2Blur.RefreshProperties();
        return mcChapter2Blur;
    }

    public McChapter2Blur()
        : base("planets/McChapter2Blur")
    {
    }

    public void Free()
    {
        StaticPool<McChapter2Blur>.Free(this);
    }
}
