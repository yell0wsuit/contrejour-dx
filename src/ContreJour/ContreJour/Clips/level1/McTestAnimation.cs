using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McTestAnimation : AnimationNode, IId
    {
        public const string ID = "level1/McTestAnimation";

        public McTestAnimation2 Layer1 { get; protected set; }

        public string Id => "level1/McTestAnimation";

        public McTestAnimation()
            : base("level1/McTestAnimation")
        {
            Layer1 = new McTestAnimation2();
            AddChild("Layer1", Layer1);
            Initialize();
        }
    }
}
