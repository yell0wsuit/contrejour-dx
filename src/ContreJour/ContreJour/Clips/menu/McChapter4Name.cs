using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McChapter4Name : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McChapter4Name";

        public string Id => "menu/McChapter4Name";

        public static McChapter4Name New()
        {
            McChapter4Name mcChapter4Name = StaticPool.New<McChapter4Name>();
            mcChapter4Name.RefreshProperties();
            return mcChapter4Name;
        }

        public McChapter4Name()
            : base("menu/McChapter4Name")
        {
        }

        public void Free()
        {
            StaticPool.Free<McChapter4Name>(this);
        }
    }
}
