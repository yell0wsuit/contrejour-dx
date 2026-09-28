using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapterLockedBlur : Sprite, IFreeable, IId
{
    public const string ID = "planets/McChapterLockedBlur";

    public string Id => "planets/McChapterLockedBlur";

    public static McChapterLockedBlur New()
    {
        McChapterLockedBlur mcChapterLockedBlur = StaticPool<McChapterLockedBlur>.New();
        mcChapterLockedBlur.RefreshProperties();
        return mcChapterLockedBlur;
    }

    public McChapterLockedBlur()
        : base("planets/McChapterLockedBlur")
    {
    }

    public void Free()
    {
        StaticPool<McChapterLockedBlur>.Free(this);
    }
}
