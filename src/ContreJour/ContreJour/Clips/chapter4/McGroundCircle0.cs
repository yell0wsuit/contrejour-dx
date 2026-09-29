using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McGroundCircle0 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McGroundCircle0";

        public string Id => "chapter4/McGroundCircle0";

        public static McGroundCircle0 New()
        {
            McGroundCircle0 mcGroundCircle = StaticPool.New<McGroundCircle0>();
            mcGroundCircle.RefreshProperties();
            return mcGroundCircle;
        }

        public McGroundCircle0()
            : base("chapter4/McGroundCircle0")
        {
        }

        public void Free()
        {
            StaticPool.Free<McGroundCircle0>(this);
        }
    }
}
