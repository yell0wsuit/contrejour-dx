using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class LystokAnimation : AnimationNode, IId
    {
        public const string ID = "level1/LystokAnimation";

        public Sprite instance5231921 { get; protected set; }

        public string Id => "level1/LystokAnimation";

        public LystokAnimation()
            : base("level1/LystokAnimation")
        {
            instance5231921 = new Sprite(ClipIds.Level1.McLystok1Content);
            AddChild("instance5231921", instance5231921);
            Initialize();
        }
    }
}
