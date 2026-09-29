using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McAchievementLock : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McAchievementLock";

        public string Id => "menu/McAchievementLock";

        public static McAchievementLock New()
        {
            McAchievementLock mcAchievementLock = StaticPool.New<McAchievementLock>();
            mcAchievementLock.RefreshProperties();
            return mcAchievementLock;
        }

        public McAchievementLock()
            : base("menu/McAchievementLock")
        {
        }

        public void Free()
        {
            StaticPool.Free<McAchievementLock>(this);
        }
    }
}
