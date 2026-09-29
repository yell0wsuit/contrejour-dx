using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallMonster : Sprite, IFreeable, IId
    {
        public const string ID = "common/McEyeBallMonster";

        public string Id => "common/McEyeBallMonster";

        public static McEyeBallMonster New()
        {
            McEyeBallMonster mcEyeBallMonster = StaticPool.New<McEyeBallMonster>();
            mcEyeBallMonster.RefreshProperties();
            return mcEyeBallMonster;
        }

        public McEyeBallMonster()
            : base("common/McEyeBallMonster")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBallMonster>(this);
        }
    }
}
