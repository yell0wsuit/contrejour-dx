using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapterLight : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapterLight";

    public string Id => "menu/McChapterLight";

    public static McChapterLight New()
    {
        McChapterLight mcChapterLight = StaticPool.New<McChapterLight>();
        mcChapterLight.RefreshProperties();
        return mcChapterLight;
    }

    public McChapterLight()
        : base("menu/McChapterLight")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapterLight>(this);
    }
}
