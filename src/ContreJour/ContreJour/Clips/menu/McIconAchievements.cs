using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McIconAchievements : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McIconAchievements";

        public string Id => "menu/McIconAchievements";

        public static McIconAchievements New()
        {
            McIconAchievements mcIconAchievements = StaticPool.New<McIconAchievements>();
            mcIconAchievements.RefreshProperties();
            return mcIconAchievements;
        }

        public McIconAchievements()
            : base("menu/McIconAchievements")
        {
        }

        public void Free()
        {
            StaticPool.Free<McIconAchievements>(this);
        }
    }
}
