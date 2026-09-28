using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStripesHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStripesHintView";

    public string Id => "chapter1/McStripesHintView";

    public static McStripesHintView New()
    {
        McStripesHintView mcStripesHintView = StaticPool.New<McStripesHintView>();
        mcStripesHintView.RefreshProperties();
        return mcStripesHintView;
    }

    public McStripesHintView()
        : base("chapter1/McStripesHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McStripesHintView>(this);
    }
}
