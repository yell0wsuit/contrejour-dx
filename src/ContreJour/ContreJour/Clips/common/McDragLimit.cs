using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McDragLimit : Sprite, IFreeable, IId
    {
        public const string ID = "common/McDragLimit";

        public string Id => "common/McDragLimit";

        public static McDragLimit New()
        {
            McDragLimit mcDragLimit = StaticPool.New<McDragLimit>();
            mcDragLimit.RefreshProperties();
            return mcDragLimit;
        }

        public McDragLimit()
            : base("common/McDragLimit")
        {
        }

        public void Free()
        {
            StaticPool.Free<McDragLimit>(this);
        }
    }
}
