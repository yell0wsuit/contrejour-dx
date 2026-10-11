using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackground2 : AnimationNode, IId
    {
        public const string ID = "level1/McBackground2";

        public Sprite huj { get; protected set; }

        public Sprite instance5232006 { get; protected set; }

        public Sprite instance5232014 { get; protected set; }

        public Sprite instance5232032 { get; protected set; }

        public Sprite instance5232034 { get; protected set; }

        public Sprite instance5232042 { get; protected set; }

        public string Id => "level1/McBackground2";

        public McBackground2()
            : base("level1/McBackground2")
        {
            huj = new Sprite(ClipIds.Level1.McBackground2Back);
            AddChild("huj", huj);
            instance5232006 = new Sprite(ClipIds.Level1.McSunBackground);
            AddChild("instance5232006", instance5232006);
            instance5232014 = new Sprite(ClipIds.Level1.McSunLight);
            AddChild("instance5232014", instance5232014);
            instance5232032 = new Sprite(ClipIds.Level1.McBlackSkyBackground);
            AddChild("instance5232032", instance5232032);
            instance5232034 = new Sprite(ClipIds.Level1.McBackground1Front);
            AddChild("instance5232034", instance5232034);
            instance5232042 = new Sprite(ClipIds.Level1.McBackground0Front);
            AddChild("instance5232042", instance5232042);
            Initialize();
        }
    }
}
