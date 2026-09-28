using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightsHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McLightsHintView";

    public string Id => "chapter1/McLightsHintView";

    public static McLightsHintView New()
    {
        McLightsHintView mcLightsHintView = StaticPool<McLightsHintView>.New();
        mcLightsHintView.RefreshProperties();
        return mcLightsHintView;
    }

    public McLightsHintView()
        : base("chapter1/McLightsHintView")
    {
    }

    public void Free()
    {
        StaticPool<McLightsHintView>.Free(this);
    }
}
