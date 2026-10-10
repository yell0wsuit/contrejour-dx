using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoseHeadFront : AnimationNode, IId
    {
        public const string ID = "level1/McRoseHeadFront";

        public Sprite instance5232242 { get; protected set; }

        public Sprite instance5232245 { get; protected set; }

        public Sprite instance5232248 { get; protected set; }

        public string Id => "level1/McRoseHeadFront";

        public McRoseHeadFront()
            : base("level1/McRoseHeadFront")
        {
            instance5232242 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232242", instance5232242);
            instance5232245 = new Sprite(ClipIds.Level1.PelustokNoLight);
            AddChild("instance5232245", instance5232245);
            instance5232248 = new Sprite(ClipIds.Level1.McRoseHeadBase2);
            AddChild("instance5232248", instance5232248);
            Initialize();
        }
    }
}
