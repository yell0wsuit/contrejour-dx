using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoseHeadDown : AnimationNode, IId
    {
        public const string ID = "level1/McRoseHeadDown";

        public McRoseHeadLightDown content { get; protected set; }

        public string Id => "level1/McRoseHeadDown";

        public McRoseHeadDown()
            : base("level1/McRoseHeadDown")
        {
            content = new McRoseHeadLightDown();
            AddChild("content", content);
            Initialize();
        }
    }
}
