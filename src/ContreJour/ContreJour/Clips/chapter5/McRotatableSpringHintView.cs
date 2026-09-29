using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRotatableSpringHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McRotatableSpringHintView";

        public string Id => "chapter5/McRotatableSpringHintView";

        public static McRotatableSpringHintView New()
        {
            McRotatableSpringHintView mcRotatableSpringHintView = StaticPool.New<McRotatableSpringHintView>();
            mcRotatableSpringHintView.RefreshProperties();
            return mcRotatableSpringHintView;
        }

        public McRotatableSpringHintView()
            : base("chapter5/McRotatableSpringHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRotatableSpringHintView>(this);
        }
    }
}
