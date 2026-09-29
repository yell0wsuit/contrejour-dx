using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoseLightBlue : Sprite, IFreeable, IId
    {
        public const string ID = "level1/McRoseLightBlue";

        public string Id => "level1/McRoseLightBlue";

        public static McRoseLightBlue New()
        {
            McRoseLightBlue mcRoseLightBlue = StaticPool.New<McRoseLightBlue>();
            mcRoseLightBlue.RefreshProperties();
            return mcRoseLightBlue;
        }

        public McRoseLightBlue()
            : base("level1/McRoseLightBlue")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRoseLightBlue>(this);
        }
    }
}
