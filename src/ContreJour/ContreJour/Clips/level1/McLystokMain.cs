using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLystokMain : AnimationNode, IId
    {
        public const string ID = "level1/McLystokMain";

        public Sprite content { get; protected set; }

        public MovieClip tear { get; protected set; }

        public string Id => "level1/McLystokMain";

        public McLystokMain()
            : base("level1/McLystokMain")
        {
            content = new Sprite(ClipIds.Level1.McLystokMainContent);
            AddChild("content", content);
            tear = new MovieClip(ClipIds.Level1.McTear);
            AddChild("tear", tear);
            Initialize();
        }
    }
}
