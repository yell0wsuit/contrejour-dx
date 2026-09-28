using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChillingoLogo : AnimationNode, IFreeable, IId
{
    public const string ID = "loading/McChillingoLogo";

    public McChillingoLogoStart startAnimation { get; protected set; }

    public McChillingoPetitCircle instance5229966 { get; protected set; }

    public McLeg instance5229968 { get; protected set; }

    public McLeg instance5229970 { get; protected set; }

    public McLeg instance5229972 { get; protected set; }

    public McLeg instance5229974 { get; protected set; }

    public string Id => "loading/McChillingoLogo";

    public static McChillingoLogo New()
    {
        McChillingoLogo mcChillingoLogo = StaticPool.New<McChillingoLogo>();
        mcChillingoLogo.RefreshProperties();
        return mcChillingoLogo;
    }

    public McChillingoLogo()
        : base("loading/McChillingoLogo")
    {
        startAnimation = new McChillingoLogoStart();
        AddChild("startAnimation", startAnimation);
        instance5229966 = new McChillingoPetitCircle();
        AddChild("instance5229966", instance5229966);
        instance5229968 = new McLeg();
        AddChild("instance5229968", instance5229968);
        instance5229970 = new McLeg();
        AddChild("instance5229970", instance5229970);
        instance5229972 = new McLeg();
        AddChild("instance5229972", instance5229972);
        instance5229974 = new McLeg();
        AddChild("instance5229974", instance5229974);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McChillingoLogo>(this);
    }
}
