using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFlyWing : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McFlyWing";

        public string Id => "chapter5/McFlyWing";

        public static McFlyWing New()
        {
            McFlyWing mcFlyWing = StaticPool.New<McFlyWing>();
            mcFlyWing.RefreshProperties();
            return mcFlyWing;
        }

        public McFlyWing()
            : base("chapter5/McFlyWing")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFlyWing>(this);
        }
    }
}
