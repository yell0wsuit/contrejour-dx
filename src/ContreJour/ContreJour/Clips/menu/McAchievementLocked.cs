using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McAchievementLocked : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McAchievementLocked";

        public string Id => "menu/McAchievementLocked";

        public static McAchievementLocked New()
        {
            McAchievementLocked mcAchievementLocked = StaticPool.New<McAchievementLocked>();
            mcAchievementLocked.RefreshProperties();
            return mcAchievementLocked;
        }

        public McAchievementLocked()
            : base("menu/McAchievementLocked")
        {
        }

        public void Free()
        {
            StaticPool.Free<McAchievementLocked>(this);
        }
    }
}
