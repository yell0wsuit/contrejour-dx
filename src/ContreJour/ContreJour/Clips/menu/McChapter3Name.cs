using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McChapter3Name : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McChapter3Name";

        public string Id => "menu/McChapter3Name";

        public static McChapter3Name New()
        {
            McChapter3Name mcChapter3Name = StaticPool.New<McChapter3Name>();
            mcChapter3Name.RefreshProperties();
            return mcChapter3Name;
        }

        public McChapter3Name()
            : base("menu/McChapter3Name")
        {
        }

        public void Free()
        {
            StaticPool.Free<McChapter3Name>(this);
        }
    }
}
