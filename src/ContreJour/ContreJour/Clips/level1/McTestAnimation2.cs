using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McTestAnimation2 : AnimationNode, IId
    {
        public const string ID = "level1/McTestAnimation2";

        public Sprite light { get; protected set; }

        public string Id => "level1/McTestAnimation2";

        public McTestAnimation2()
            : base("level1/McTestAnimation2")
        {
            light = new Sprite(ClipIds.Level1.McRoseLightBlue2);
            AddChild("light", light);
            Initialize();
        }
    }
}
