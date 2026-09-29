using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter1Blur : Sprite, IFreeable, IId
{
    public const string ID = "planets/McChapter1Blur";

    public string Id => "planets/McChapter1Blur";

    public static McChapter1Blur New()
    {
        McChapter1Blur mcChapter1Blur = StaticPool.New<McChapter1Blur>();
        mcChapter1Blur.RefreshProperties();
        return mcChapter1Blur;
    }

    public McChapter1Blur()
        : base("planets/McChapter1Blur")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapter1Blur>(this);
    }
}
