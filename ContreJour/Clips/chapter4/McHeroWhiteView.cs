using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHeroWhiteView : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McHeroWhiteView";

    public string Id => "chapter4/McHeroWhiteView";

    public static McHeroWhiteView New()
    {
        McHeroWhiteView mcHeroWhiteView = StaticPool.New<McHeroWhiteView>();
        mcHeroWhiteView.RefreshProperties();
        return mcHeroWhiteView;
    }

    public McHeroWhiteView()
        : base("chapter4/McHeroWhiteView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McHeroWhiteView>(this);
    }
}
