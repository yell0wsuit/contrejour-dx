using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McMoreTentaclesHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter1/McMoreTentaclesHintView";

        public string Id => "chapter1/McMoreTentaclesHintView";

        public static McMoreTentaclesHintView New()
        {
            McMoreTentaclesHintView mcMoreTentaclesHintView = StaticPool.New<McMoreTentaclesHintView>();
            mcMoreTentaclesHintView.RefreshProperties();
            return mcMoreTentaclesHintView;
        }

        public McMoreTentaclesHintView()
            : base("chapter1/McMoreTentaclesHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McMoreTentaclesHintView>(this);
        }
    }
}
