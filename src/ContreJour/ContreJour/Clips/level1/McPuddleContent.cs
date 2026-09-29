using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPuddleContent : Sprite, IFreeable, IId
    {
        public const string ID = "level1/McPuddleContent";

        public string Id => "level1/McPuddleContent";

        public static McPuddleContent New()
        {
            McPuddleContent mcPuddleContent = StaticPool.New<McPuddleContent>();
            mcPuddleContent.RefreshProperties();
            return mcPuddleContent;
        }

        public McPuddleContent()
            : base("level1/McPuddleContent")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPuddleContent>(this);
        }
    }
}
