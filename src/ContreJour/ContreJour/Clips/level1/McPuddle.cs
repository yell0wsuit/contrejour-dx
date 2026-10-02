using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPuddle : AnimationNode, IId
    {
        public const string ID = "level1/McPuddle";

        public Sprite instance5232114 { get; protected set; }

        public string Id => "level1/McPuddle";

        public McPuddle()
            : base("level1/McPuddle")
        {
            instance5232114 = new Sprite(ClipIds.Level1.McPuddleContent);
            AddChild("instance5232114", instance5232114);
            Initialize();
        }
    }
}
