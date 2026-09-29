using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McDragView_5 : Sprite, IFreeable, IId
    {
        public const string ID = "common/McDragView_5";

        public string Id => "common/McDragView_5";

        public static McDragView_5 New()
        {
            McDragView_5 mcDragView_ = StaticPool.New<McDragView_5>();
            mcDragView_.RefreshProperties();
            return mcDragView_;
        }

        public McDragView_5()
            : base("common/McDragView_5")
        {
        }

        public void Free()
        {
            StaticPool.Free<McDragView_5>(this);
        }
    }
}
