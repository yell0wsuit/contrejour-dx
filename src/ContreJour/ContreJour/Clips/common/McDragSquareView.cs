using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McDragSquareView : Sprite, IFreeable, IId
    {
        public const string ID = "common/McDragSquareView";

        public string Id => "common/McDragSquareView";

        public static McDragSquareView New()
        {
            McDragSquareView mcDragSquareView = StaticPool.New<McDragSquareView>();
            mcDragSquareView.RefreshProperties();
            return mcDragSquareView;
        }

        public McDragSquareView()
            : base("common/McDragSquareView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McDragSquareView>(this);
        }
    }
}
