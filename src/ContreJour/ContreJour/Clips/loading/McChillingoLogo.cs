using System.CodeDom.Compiler;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McChillingoLogo : AnimationNode, IId
    {
        public const string ID = "loading/McChillingoLogo";

        public MovieClip startAnimation { get; protected set; }

        public Sprite instance5229966 { get; protected set; }

        public Sprite instance5229968 { get; protected set; }

        public Sprite instance5229970 { get; protected set; }

        public Sprite instance5229972 { get; protected set; }

        public Sprite instance5229974 { get; protected set; }

        public string Id => "loading/McChillingoLogo";

        public McChillingoLogo()
            : base("loading/McChillingoLogo")
        {
            startAnimation = new MovieClip(ClipIds.Loading.McChillingoLogoStart);
            AddChild("startAnimation", startAnimation);
            instance5229966 = new Sprite(ClipIds.Loading.McChillingoPetitCircle);
            AddChild("instance5229966", instance5229966);
            instance5229968 = new Sprite(ClipIds.Loading.McLeg);
            AddChild("instance5229968", instance5229968);
            instance5229970 = new Sprite(ClipIds.Loading.McLeg);
            AddChild("instance5229970", instance5229970);
            instance5229972 = new Sprite(ClipIds.Loading.McLeg);
            AddChild("instance5229972", instance5229972);
            instance5229974 = new Sprite(ClipIds.Loading.McLeg);
            AddChild("instance5229974", instance5229974);
            Initialize();
        }
    }
}
