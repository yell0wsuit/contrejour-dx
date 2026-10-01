using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoseHeadLightDown : AnimationNode, IId
    {
        public const string ID = "level1/McRoseHeadLightDown";

        public Sprite instance5232203 { get; protected set; }

        public Sprite instance5232206 { get; protected set; }

        public Sprite instance5232209 { get; protected set; }

        public Sprite instance5232212 { get; protected set; }

        public Sprite light { get; protected set; }

        public Sprite instance5232218 { get; protected set; }

        public Sprite instance5232221 { get; protected set; }

        public string Id => "level1/McRoseHeadLightDown";

        public McRoseHeadLightDown()
            : base("level1/McRoseHeadLightDown")
        {
            instance5232203 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232203", instance5232203);
            instance5232206 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232206", instance5232206);
            instance5232209 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232209", instance5232209);
            instance5232212 = new Sprite(ClipIds.Level1.McRoseHeadBase);
            AddChild("instance5232212", instance5232212);
            light = new Sprite(ClipIds.Level1.McRoseLightBlue);
            AddChild("light", light);
            instance5232218 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232218", instance5232218);
            instance5232221 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232221", instance5232221);
            Initialize();
        }
    }
}
